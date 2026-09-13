using JobTracker.Application.Interfaces.Repository;
using JobTracker.Domain.Entities;
using JobTracker.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Repositories
{
    public class RefreshTokenRepository
        : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(
                refreshToken);

            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(
            string tokenHash)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(x =>
                    x.TokenHash == tokenHash);
        }

        public async Task UpdateAsync(
            RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);

            await _context.SaveChangesAsync();
        }
    }
}