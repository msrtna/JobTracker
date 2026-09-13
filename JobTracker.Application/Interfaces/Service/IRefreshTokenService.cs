namespace JobTracker.Application.Interfaces.Service
{
    public interface IRefreshTokenService
    {
        string GenerateToken();
        string HashToken(string token);
        DateTime GetExpiration();
    }
}
