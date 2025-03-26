using FirebaseAdmin.Auth;
using HealthyCareAssistant.Contact.Repo.Entity;
using HealthyCareAssistant.Contact.Repo.IUOW;
using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.Core.Store;
using HealthyCareAssistant.ModelViews.AuthModelViews;
using HealthyCareAssistant.Service.Config;
using HealthyCareAssistant.Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HealthyCareAssistant.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class authController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<User> _userRepo;
        private readonly AuthService _authService;
        public authController(IUserService userService, IConfiguration configuration, IUnitOfWork unitOfWork, IGenericRepository<User> userRepo, AuthService authService)
        {
            _userService = userService;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _userRepo = userRepo;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModelViews model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RegisterAsync(model);
            return Ok(new { message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModelViews model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            var token = await _userService.LoginAsync(model);

            // 🔹 Đảm bảo token hợp lệ (loại bỏ trường hợp token chứa thông báo lỗi)
            if (string.IsNullOrEmpty(token) || token == "Email hoặc mật khẩu không đúng")
            {
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });
            }

            return Ok(new
            {
                message = "Đăng nhập thành công",
                token
            });
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] TokenRqGoogleLogin model)
        {
            var token = await _userService.LoginWithGoogleAsync(model.IdToken);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Token không hợp lệ" });
            }

            return Ok(new { message = "Đăng nhập thành công", token });
        }

        [HttpPost("generate-token")]
        public async Task<IActionResult> GenerateToken([FromBody] LoginRequest request)
        {
            try
            {
                var idToken = await _authService.SignInWithEmailAndPasswordAsync(request.Email, request.Password);
                return Ok(new { idToken });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Lỗi đăng nhập Firebase", error = ex.Message });
            }
        }
        //[HttpPost("logout")]
        //[Authorize]
        //public IActionResult Logout()
        //{
        //    // Có thể thêm logic xóa token khỏi DB (nếu cần)
        //    return Ok(new { message = "Đăng xuất thành công" });
        //}
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest model)
        {
            var user = _userRepo.Entities.FirstOrDefault(u => u.Email == model.Email);
            if (user == null || string.IsNullOrEmpty(user.RefreshToken) || user.RefreshToken != model.refreshToken)
            {
                return Unauthorized(new { message = "Invalid refresh token." });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]);
            try
            {
                // 🔹 Xác thực Token hiện tại xem có hết hạn chưa
                var claimsPrincipal = tokenHandler.ValidateToken(user.RefreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    ValidateLifetime = true
                }, out var validatedToken);

                return Ok(new { token = user.RefreshToken });
            }
            catch (SecurityTokenExpiredException)
            {
                //  Token đã hết hạn -> Cấp Token mới
                var role = user.Role?.RoleName ?? "User";
                var permissions = new List<string>();
                var newToken = TokenHelper.GenerateJwtToken(
                    user,
                    _configuration["JwtSettings:Secret"],
                    _configuration["JwtSettings:Issuer"],
                    _configuration["JwtSettings:Audience"],
                    Convert.ToInt32(_configuration["JwtSettings:ExpiryMinutes"])
                );

                //  Cập nhật Token mới trong DB
                user.RefreshToken = newToken;
                await _userRepo.UpdateAsync(user);
                await _unitOfWork.SaveAsync();

                return Ok(new { token = newToken });
            }
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var result = await _userService.ForgotPasswordAsync(model.Email);
            if (!result) return NotFound(new { message = "Email không tồn tại trong hệ thống" });

            return Ok(new { message = "Mã OTP đã được gửi qua email" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var result = await _userService.ResetPasswordAsync(model);
            if (!result) return BadRequest(new { message = "OTP không hợp lệ hoặc mật khẩu không khớp" });

            return Ok(new { message = "Mật khẩu đã được đặt lại thành công" });
        }


        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var userRoleName = User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";
            var userPhoneNumber = User.FindFirstValue("phone_number") ?? "N/A";

            Console.WriteLine($"[JWT] Authenticated User - ID: {userId}, Email: {userEmail}, Role: {userRoleName}");

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("[JWT] Token is invalid (userId is null).");
                return Unauthorized(new { message = "Invalid Token" });
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                Console.WriteLine("[JWT] User not found in the database.");
                return NotFound(new { message = "User not found" });
            }

            var extendedUserInfo = new
            {
                user.Name,
                user.Email,
                user.PhoneNumber,
                Role = userRoleName,
            };

            Console.WriteLine($"[JWT] Successful authentication - User: {user.Email}");
            return Ok(extendedUserInfo);
        }


    }
}
