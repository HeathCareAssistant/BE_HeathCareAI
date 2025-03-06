using HealthyCareAssistant.Contact.Repo.Entity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HealthyCareAssistant.Service.Config
{
    public static class TokenHelper
    {
        public static string GenerateJwtToken(User user, string? role, List<string>? permissions, string key, string issuer, string audience, int expiryMinutes)
        {
            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Invalid user data or secret key.");
            }

            if (expiryMinutes <= 0)
            {
                throw new ArgumentException("Expiry time must be greater than 0.");
            }

            // Đảm bảo Role chỉ có giá trị hợp lệ
            var normalizedRole = role?.Trim().ToLower() switch
            {
                "admin" => "Admin",
                "user" => "User",
                _ => "User" // Mặc định là "User" nếu giá trị không hợp lệ
            };

            // Danh sách quyền (permissions) không bắt buộc
            var validPermissions = permissions ?? new List<string>();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, normalizedRole)
            };

            // Thêm permissions nếu có
            claims.AddRange(validPermissions.Select(permission => new Claim("Permission", permission)));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine($"[JWT]  Token Generated: {jwt}");

            return jwt;
        }
    }
}
