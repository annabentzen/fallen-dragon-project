using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DragonGame.Models;
using Microsoft.IdentityModel.Tokens;

namespace DragonGame.Services
{
    /// <summary>
    /// Service for generating and validating JWT tokens
    /// </summary>
    public interface IJwtService
    {
        string GenerateToken(User user);
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generates a JWT token for the authenticated user
        /// Token contains user ID and username as claims
        /// </summary>
        public string GenerateToken(User user)
        {
            // Get secret key from appsettings.json
            var secretKey = _configuration["Jwt:SecretKey"] 
                ?? throw new InvalidOperationException("JWT Secret Key not configured");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create claims (user information stored in token)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            };

            // Create token with 7 day expiration
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}