using HealthyCareAssistant.Contact.Repo.Entity;
using HealthyCareAssistant.Contact.Repo.IUOW;
using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.AuthModelViews;
using HealthyCareAssistant.ModelViews.UserModelViews;
using HealthyCareAssistant.Repo.Context;
using HealthyCareAssistant.Service.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using MailKit;
using HealthyCareAssistant.ModelViews.MailModelViews;
using HealthyCareAssistant.Service.Service.firebase;
using FirebaseAdmin.Auth;

namespace HealthyCareAssistant.Service.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<User> _userRepo;
        private readonly ILogger<UserService> _logger;
        private readonly HealthCareAssistantContext _context;
        private readonly Contract.Service.Interface.IMailService _mailService;
        private readonly IOTPService _otpService;
        private readonly FirebaseAuthService _firebaseAuthService;

        public UserService(IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IGenericRepository<User> userRepo,
            ILogger<UserService> logger,
            Contract.Service.Interface.IMailService mailService,
            IOTPService otpService,
            FirebaseAuthService firebaseAuthService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _configuration = configuration;
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _firebaseAuthService = firebaseAuthService ?? throw new ArgumentNullException(nameof(firebaseAuthService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService;
            _otpService = otpService;
        }

        public async Task<string?> LoginAsync(LoginModelViews model)
        {
            var user = _userRepo.Entities
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == model.Email && u.Status == true);

            if (user == null || user.PasswordHash != HashHelper.ComputeSha256Hash(model.Password))
            {
                return "Email hoặc mật khẩu không đúng";
            }

            var token = TokenHelper.GenerateJwtToken(
                user,
                _configuration["JwtSettings:Secret"],
                _configuration["JwtSettings:Issuer"],
                _configuration["JwtSettings:Audience"],
                Convert.ToInt32(_configuration["JwtSettings:ExpiryMinutes"]));

            user.RefreshToken = token;
            await _userRepo.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return token;
        }

        public async Task<string?> LoginWithGoogleAsync(string idToken)
        {
            if (string.IsNullOrEmpty(idToken)) throw new ArgumentException("Token không được để trống.");

            try
            {
                var decodedToken = await _firebaseAuthService.VerifyIdTokenAsync(idToken);
                var email = decodedToken.Claims.ContainsKey("email") ? decodedToken.Claims["email"].ToString() : "No Email";
                var userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
                var displayName = userRecord.DisplayName ?? email;

                var user = _userRepo.Entities.FirstOrDefault(u => u.Email == email);

                if (user == null)
                {
                    user = new User
                    {
                        Name = displayName,
                        Email = email,
                        RoleId = 2,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Status = true
                    };

                    await _userRepo.InsertAsync(user);
                    await _unitOfWork.SaveAsync();
                }
                else if (user.Status != true)
                {
                    return "Tài khoản đã bị khóa.";
                }

                return TokenHelper.GenerateJwtToken(
                    user,
                    _configuration["JwtSettings:Secret"],
                    _configuration["JwtSettings:Issuer"],
                    _configuration["JwtSettings:Audience"],
                    Convert.ToInt32(_configuration["JwtSettings:ExpiryMinutes"]));
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi xác thực token từ Firebase: {0}", ex.Message);
                return null;
            }
        }

        public async Task<string> RegisterAsync(RegisterModelViews model)
        {
            int assignedRoleId = (model.RoleId == 1 || model.RoleId == 2) ? model.RoleId : 2;

            var role = await _unitOfWork.GetRepository<Role>().Entities.FirstOrDefaultAsync(r => r.RoleId == assignedRoleId);
            if (role == null) return $"Lỗi: RoleID {assignedRoleId} không tồn tại trong hệ thống.";

            var existingUser = _userRepo.Entities.FirstOrDefault(u => u.Email == model.Email);
            if (existingUser != null) return "Email already exists.";

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                PasswordHash = HashHelper.ComputeSha256Hash(model.Password),
                PhoneNumber = model.PhoneNumber,
                RoleId = model.RoleId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = true
            };

            await _userRepo.InsertAsync(user);
            await _unitOfWork.SaveAsync();

            return "User registered successfully.";
        }

        public async Task<(IEnumerable<UserModelView> users, int totalElement, int totalPage)> GetAllUsersPaginatedAsync(int page, int pageSize)
        {
            var totalElement = await _userRepo.Entities.Where(u => u.Status == true && u.RoleId == 2).CountAsync();
            var totalPage = (int)Math.Ceiling(totalElement / (double)pageSize);

            var users = await _userRepo.Entities
                .Where(u => u.Status == true && u.RoleId == 2)
                .Include(u => u.Role)
                .OrderBy(u => u.UserId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserModelView
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    Fcmtoken = u.Fcmtoken,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role != null ? u.Role.RoleName : null,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync();

            return (users, totalElement, totalPage);
        }

        public async Task<UserModelView> GetUserByIdAsync(int userId)
        {
            var user = await _userRepo.Entities
                .FirstOrDefaultAsync(u => u.UserId == userId && u.RoleId == 2);

            if (user == null || user.Status != true)
            {
                return null; 
            }

            return new UserModelView
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Fcmtoken = user.Fcmtoken,
                CreatedAt = user.CreatedAt
            };
        }


        public async Task<string> CreateUserAsync(UserCreateRequest request)
        {
            var existingUser = await _userRepo.Entities.AnyAsync(u => u.Email == request.Email);
            if (existingUser) return "Email đã tồn tại.";

            var newUser = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = HashHelper.ComputeSha256Hash(request.Password),
                PhoneNumber = request.PhoneNumber,
                Fcmtoken = request.Fcmtoken,
                RoleId = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = true
            };

            await _userRepo.InsertAsync(newUser);
            await _unitOfWork.SaveAsync();
            return "User tạo thành công.";
        }

        public async Task<string> UpdateUserAsync(int userId, UserUpdateRequest request)
        {
            var user = await _userRepo.Entities.FirstOrDefaultAsync(u => u.UserId == userId && u.RoleId == 2);
            if (user == null) return "Không tìm thấy user.";
            if (user.Status != true) return "User đã bị xóa.";

            user.Name = request.Name;
            user.PhoneNumber = request.PhoneNumber;
            user.Fcmtoken = request.Fcmtoken;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
            return "User cập nhật thành công.";
        }

        public async Task<string> DeleteUserAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null || user.RoleId != 2) return "Không tìm thấy user.";
            if (user.Status != true) return "User đã bị xóa.";

            user.Status = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return "User đã bị xoá.";
        }

        public async Task<IEnumerable<UserModelView>> SearchUsersAsync(string keyword)
        {
            return await _userRepo.Entities
                .Where(u => u.RoleId == 2 && u.Status == true &&
                           (u.Name.Contains(keyword) || u.PhoneNumber.Contains(keyword)))
                .Select(u => new UserModelView
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Fcmtoken = u.Fcmtoken,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userRepo.Entities.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user.Status != true) return false;

            string otp = _otpService.GenerateOtp();
            await _otpService.StoreOtpAsync(email, otp);

            string userName = string.IsNullOrWhiteSpace(user.Name) ? "User" : user.Name;

            var mailData = new EmailData
            {
                EmailToId = email,
                EmailToName = userName,
                EmailSubject = "Mã OTP đặt lại mật khẩu",
                EmailBody = $"Mã OTP của bạn là: <b>{otp}</b>. Mã này có hiệu lực trong 5 phút."
            };

            return await _mailService.SendEmailAsync(mailData);
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordModel model)
        {
            var user = await _userRepo.Entities.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || user.Status != true) return false;

            bool isOtpValid = await _otpService.ValidateOtpAsync(model.Email, model.Otp);
            if (!isOtpValid) return false;

            user.PasswordHash = HashHelper.ComputeSha256Hash(model.NewPassword);
            await _userRepo.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<bool> ValidatePasswordAsync(User user, string inputPassword)
        {
            string hashedInput = HashHelper.ComputeSha256Hash(inputPassword);
            return hashedInput == user.PasswordHash;
        }

        public async Task<UserModelView?> GetUserByIdAsync(string userId)
        {
            var user = await _userRepo.Entities
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId.ToString() == userId && u.Status == true);

            return user == null ? null : new UserModelView(user);
        }
    }
}
