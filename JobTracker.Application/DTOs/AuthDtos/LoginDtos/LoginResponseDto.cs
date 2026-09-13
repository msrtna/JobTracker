using JobTracker.Application.DTOs.AuthDtos.UserDtos;

namespace JobTracker.Application.DTOs.AuthDtos.LoginDtos
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; } = null!;
    }
}
