using System.Security.Cryptography;
using System.Text;
using JobTracker.Application.Interfaces.Service;
using Microsoft.Extensions.Configuration;

namespace JobTracker.Infrastructure.Security
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IConfiguration _configuration;

        public RefreshTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public string GenerateToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(randomBytes);
        }

        public string HashToken(string token)
        {
            var tokenBytes = Encoding.UTF8.GetBytes(token);

            var hashBytes = SHA256.HashData(tokenBytes);

            return Convert.ToHexString(hashBytes);
        }

        public DateTime GetExpiration()
        {
            var expirationDays =
                int.Parse(
                    _configuration
                    .GetSection("Jwt")
                    ["RefreshTokenExpirationDays"]!);

            return DateTime.UtcNow.AddDays(
                expirationDays);
        }
    }
}