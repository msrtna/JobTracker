using JobTracker.Domain.Entities;

namespace JobTracker.Application.Interfaces.Repository
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refreshToken);

        Task<RefreshToken?> GetByTokenHashAsync(
            string tokenHash);

        Task UpdateAsync(RefreshToken refreshToken);
    }
}
