using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JobTracker.Application.Interfaces.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JobTracker.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public string GenerateToken(long userId, string email)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expirationMinutes =
                int.Parse(jwtSettings["ExpirationMinutes"]!);

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    userId.ToString()),

                new Claim(
                    ClaimTypes.Email,
                    email)
            };

            var expiration = DateTime.UtcNow
                .AddMinutes(expirationMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public DateTime GetExpiration()
        {
            var expirationMinutes =
                int.Parse(
                    _configuration
                        .GetSection("Jwt")["ExpirationMinutes"]!);

            return DateTime.UtcNow
                .AddMinutes(expirationMinutes);
        }
    }
}
