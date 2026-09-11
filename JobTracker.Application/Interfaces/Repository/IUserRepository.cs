using JobTracker.Domain.Entities;

namespace JobTracker.Application.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task CreateAsync(User user);
        Task<bool> ExistsByEmailAsync(string email);
    }
}
