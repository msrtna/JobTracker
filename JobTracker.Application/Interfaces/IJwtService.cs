namespace JobTracker.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(long userId, string email);
        DateTime GetExpiration();
    }
}
