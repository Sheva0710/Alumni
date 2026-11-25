using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Alumni.Application.Interfaces;
using Alumni.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Alumni.API.Common
{
    /// <summary>
    /// İmzalı JWT (header.payload.signature) üreten servis.
    /// Payload yalnızca gizli olmayan kimlik bilgilerini (id, name, email, role) içerir.
    /// </summary>
    public class JwtTokenService : IJwtTokenService
    {
        public const string AuthCookieName = "alumni_auth_token";
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var secretKey = _configuration["Jwt:SecretKey"]
                ?? "AlumniTrackingSystem-SuperSecret-JWT-SigningKey-2026!";
            var issuer = _configuration["Jwt:Issuer"] ?? "AlumniTrackingSystem";
            var audience = _configuration["Jwt:Audience"] ?? "AlumniTrackingSystemUsers";
            var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationMinutes", 120);

            var fullName = $"{user.FirstName} {user.LastName}".Trim();

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new("id", user.Id.ToString()),
                new("name", fullName),
                new("email", user.Email),
                new("role", user.Role),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
