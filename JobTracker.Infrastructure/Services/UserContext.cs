using System.Security.Claims;
using JobTracker.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace JobTracker.Infrastructure.Services
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public long UserId
        {
            get
            {
                var userId =
                    _httpContextAccessor.HttpContext?
                        .User
                        .FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException(
                        "User is not authenticated.");
                }

                if (!long.TryParse(userId, out var parsedUserId))
                {
                    throw new UnauthorizedAccessException(
                        "Invalid user id.");
                }

                return parsedUserId;
            }
        }
    }
}
