using HealthyCareAssistant.Contact.Repo.Entity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HealthyCareAssistant.Service.Config
{
    public static class TokenHelper
    {
        public static string GenerateJwtToken(User user, string key, string issuer, string audience, int expiryMinutes)
        {
            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Invalid user data or secret key.");
            }

            if (expiryMinutes <= 0)
            {
                throw new ArgumentException("Expiry time must be greater than 0.");
            }

       
            string normalizedRole = user.Role?.RoleName?.Trim();

            var claims = new List<Claim>
            {
                new Claim("id", user.UserId.ToString()), // User ID
                new Claim("name", user.Name), // Username
                new Claim("email", user.Email) // Email
            };

 
            if (!string.IsNullOrEmpty(normalizedRole))
            {
                claims.Add(new Claim("role", normalizedRole)); 
            }

            claims.Add(new Claim("exp", DateTimeOffset.UtcNow.AddMinutes(expiryMinutes).ToUnixTimeSeconds().ToString())); // Expiration time

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
