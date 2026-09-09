using JobTracker.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace JobTracker.Infrastructure.Services
{
    public class PasswordHasherService : IPasswordHasher
    {
        private readonly PasswordHasher<object> _passwordHasher = new();

        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(new object(), password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            var result = _passwordHasher.VerifyHashedPassword(new object(), passwordHash, password);

            return result == PasswordVerificationResult.Success ||
                   result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
