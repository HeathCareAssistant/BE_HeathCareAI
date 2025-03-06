using HealthyCareAssistant.Contact.Repo.Entity;
using HealthyCareAssistant.Contact.Repo.IUOW;
using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.Core.Base;
using HealthyCareAssistant.ModelViews.AuthModelViews;
using HealthyCareAssistant.ModelViews.DrugModelViews;
using HealthyCareAssistant.ModelViews.UserModelViews;
using HealthyCareAssistant.Repo.Context;

using HealthyCareAssistant.ModelViews.UserModelViews;
using HealthyCareAssistant.Repo.Context;
using HealthyCareAssistant.Service.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace HealthyCareAssistant.Service.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserService> _logger;
        private readonly HealthCareAssistantContext _context;

        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration, IGenericRepository<User> userRepo, IEmailService emailService, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork), "UnitOfWork không được null.");
            _configuration = configuration;
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo), "User repository không được null.");
            _emailService = emailService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger không được null.");

        }

        public async Task<string?> LoginAsync(LoginModelViews model)
        {
            var userRepo = _unitOfWork.GetRepository<User>();

            // Kiểm tra user có tồn tại không
            var user = _userRepo.Entities
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == model.Email);
            if (user == null || user.PasswordHash != HashHelper.ComputeSha256Hash(model.Password))
            {
                return "Email hoặc mật khẩu không đúng";
            }

            // Lấy thông tin vai trò
            var role = user.RoleId == 1 ? "Admin" : "User";

            var roleName = user.Role?.RoleName ?? "User";
            // Lấy danh sách quyền của người dùng (nếu có)
            var permissions = new List<string>(); // Nếu có logic permission, lấy từ DB

            // Tạo token bằng TokenHelper
            var token = TokenHelper.GenerateJwtToken(
                          user,
                          _configuration["JwtSettings:Secret"],
                          _configuration["JwtSettings:Issuer"],
                          _configuration["JwtSettings:Audience"],
                          Convert.ToInt32(_configuration["JwtSettings:ExpiryMinutes"])
             );

            user.RefreshToken = token;
            await _userRepo.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return token;
        }


        public async Task<string> RegisterAsync(RegisterModelViews model)
        {
            // Chỉ chấp nhận RoleID 1 (Admin) hoặc 2 (User), mặc định là User
            int assignedRoleId = (model.RoleId == 1 || model.RoleId == 2) ? model.RoleId : 2;

            // Kiểm tra xem Role có tồn tại không
            var role = await _unitOfWork.GetRepository<Role>().Entities
                .FirstOrDefaultAsync(r => r.RoleId == assignedRoleId);

            if (role == null)
            {
                return $"Lỗi: RoleID {assignedRoleId} không tồn tại trong hệ thống.";
            }
            var userRepo = _unitOfWork.GetRepository<User>();

            // Kiểm tra Email đã tồn tại chưa
            var existingUser = _userRepo.Entities.FirstOrDefault(u => u.Email == model.Email);
            if (existingUser != null)
            {
                return "Email already exists.";
            }

            // Hash mật khẩu
            string hashedPassword = HashHelper.ComputeSha256Hash(model.Password);

            // Tạo user mới
            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                PasswordHash = hashedPassword,
                PhoneNumber = model.PhoneNumber,
                RoleId = model.RoleId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await userRepo.InsertAsync(user);
            await _unitOfWork.SaveAsync();

            return "User registered successfully.";
        }


        public async Task<(IEnumerable<UserModelView> users, int totalElement, int totalPage)> GetAllUsersPaginatedAsync(int page, int pageSize)
        {
            var totalElement = await _userRepo.Entities.CountAsync(); 
            var totalPage = (int)Math.Ceiling(totalElement / (double)pageSize);

            var users = await _userRepo.Entities
                .OrderBy(d => d.UserId) 
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new UserModelView
                {
                    UserId = d.UserId,
                    Name = d.Name,
                    Email = d.Email,
                    Fcmtoken = d.Fcmtoken,
                    PhoneNumber = d.PhoneNumber,
                    CreatedAt = d.CreatedAt,
                })
                .ToListAsync();

            return (users, totalElement, totalPage);
        }



        // 2️ Lấy User theo ID
        public async Task<UserModelView> GetUserByIdAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null || user.RoleId != 2) return null;

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

        // 3️ Tạo User mới
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
                RoleId = 2, // Mặc định là User
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepo.InsertAsync(newUser);
            await _unitOfWork.SaveAsync();
            return "User tạo thành công.";
        }

        // 4️ Cập nhật User
        public async Task<string> UpdateUserAsync(int userId, UserUpdateRequest request)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null || user.RoleId != 2) return "Không tìm thấy user.";

            user.Name = request.Name;
            user.PhoneNumber = request.PhoneNumber;
            user.Fcmtoken = request.Fcmtoken;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
            return "User cập nhật thành công.";
        }

        // 5️ Xóa User
        public async Task<string> DeleteUserAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null || user.RoleId != 2) return "Không tìm thấy user.";

            await _userRepo.DeleteAsync(userId);
            await _unitOfWork.SaveAsync();
            return "User đã bị xóa.";
        }

        public async Task<IEnumerable<UserModelView>> SearchUsersAsync(string keyword)
        {
            return await _userRepo.Entities
                .Where(u => u.RoleId == 2 &&
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
            if (user == null)
            {
                return false;
            }

            // 🔹 Generate OTP
            var otp = GenerateOtp();
            user.Otp = otp;
            await _userRepo.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            // 🔹 Send OTP via email using FluentEmail
            var emailMetadata = new EmailMetadata(
                email,
                "Password Reset OTP",
                $"Your OTP code is: <b>{otp}</b>. This code will expire in 10 minutes."
            );

            return await _emailService.SendEmailAsync(emailMetadata);
        }


        public async Task<bool> ResetPasswordAsync(ResetPasswordModel model)
        {
            var user = await _userRepo.Entities.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || user.Otp != model.Otp)
            {
                Console.WriteLine("[ResetPassword] OTP is invalid or user not found.");
                return false;
            }

            user.PasswordHash = HashHelper.ComputeSha256Hash(model.NewPassword);
            user.Otp = null; // Xóa OTP sau khi đặt lại mật khẩu
            await _userRepo.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return true;
        }



        public string GenerateOtp()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var byteArray = new byte[4];
                rng.GetBytes(byteArray);
                var otp = BitConverter.ToUInt32(byteArray, 0) % 1000000; // Giới hạn OTP trong 6 chữ số
                var otpString = otp.ToString("D6"); // Định dạng luôn là 6 chữ số

                Console.WriteLine($"[OTP] Generated OTP: {otpString}");
                return otpString;
            }
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
                .FirstOrDefaultAsync(u => u.UserId.ToString() == userId);

            return user == null ? null : new UserModelView(user);
        }

    }
}

