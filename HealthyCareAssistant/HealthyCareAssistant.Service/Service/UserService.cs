using HealthyCareAssistant.Contact.Repo.Entity;
using HealthyCareAssistant.Contact.Repo.IUOW;
using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.AuthModelViews;
using HealthyCareAssistant.Service.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Service.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<User> _userRepo;

        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration, IGenericRepository<User> userRepo)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork), "UnitOfWork không được null.");
            _configuration = configuration;
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo), "User repository không được null.");
        }

        public async Task<string?> LoginAsync(LoginModelViews model)
        {
            var userRepo = _unitOfWork.GetRepository<User>();

            // Kiểm tra user có tồn tại không
            var user = _userRepo.Entities.FirstOrDefault(u => u.Email == model.Email);
            if (user == null || user.PasswordHash != HashHelper.ComputeSha256Hash(model.Password))
            {
                return "Email hoặc mật khẩu không đúng";
            }

            // Lấy thông tin vai trò
            var role = user.Role?.RoleName ?? "Customer"; // Mặc định là Customer nếu không tìm thấy Role

            // Lấy danh sách quyền của người dùng (nếu có)
            var permissions = new List<string>(); // Nếu có logic permission, lấy từ DB

            // Tạo token bằng TokenHelper
            var token = TokenHelper.GenerateJwtToken(
                user,
                role,
                permissions,
                _configuration["JwtSettings:Secret"],
                _configuration["JwtSettings:Issuer"],
                _configuration["JwtSettings:Audience"],
                Convert.ToInt32(_configuration["JwtSettings:ExpiryMinutes"])
            );

            return token;
        }


        public async Task<string> RegisterAsync(RegisterModelViews model)
        {
            // Nếu RoleID là null hoặc 0, gán mặc định là Customer (RoleID = 3)
            int assignedRoleId = model.RoleId >= 0 ? model.RoleId : 3;

            // Kiểm tra xem Role có tồn tại không
            var roles = await _unitOfWork.GetRepository<Role>().Entities.ToListAsync();
            var role = roles.FirstOrDefault(r => r.RoleId == assignedRoleId);

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
    }
}
