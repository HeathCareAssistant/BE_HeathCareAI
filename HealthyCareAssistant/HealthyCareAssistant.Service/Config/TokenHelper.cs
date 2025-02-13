using HealthyCareAssistant.Contact.Repo.Entity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HealthyCareAssistant.Service.Config
{
    /// <summary>
    /// Helper class to generate JWT tokens.
    /// </summary>
    public static class TokenHelper
    {
        /// <summary>
        /// Generates a JWT token for a given user.
        /// </summary>
        /// <param name="user">The user for whom the token is generated.</param>
        /// <param name="role">The user's role.</param>
        /// <param name="permissions">List of permissions assigned to the user.</param>
        /// <param name="key">The secret key used to sign the token.</param>
        /// <param name="issuer">The issuer of the token.</param>
        /// <param name="audience">The audience of the token.</param>
        /// <param name="expiryMinutes">Expiration time in minutes.</param>
        /// <returns>The generated JWT token.</returns>
        public static string GenerateJwtToken(User user, string role, List<string> permissions, string key, string issuer, string audience, int expiryMinutes)
        {
            // Define the claims for the JWT token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role)
            };

            // Add permissions as claims
            claims.AddRange(permissions.Select(permission => new Claim("Permission", permission)));

            // Generate the security key from the secret key
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create the JWT token
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
