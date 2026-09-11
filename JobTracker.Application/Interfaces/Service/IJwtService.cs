namespace JobTracker.Application.Interfaces.Service
{
    public interface IJwtService
    {
        string GenerateToken(long userId, string email);
        DateTime GetExpiration();
    }
}
