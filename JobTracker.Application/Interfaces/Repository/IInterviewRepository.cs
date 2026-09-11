using JobTracker.Domain.Entities;

namespace JobTracker.Application.Interfaces.Repository
{
    public interface IInterviewRepository
    {
        Task<List<Interview>> GetAllAsync();
        Task<Interview?> GetByIdAsync(long id);
        Task CreateAsync(Interview interview);
        Task UpdateAsync(Interview interview);
        Task DeleteAsync(Interview interview);
        Task<List<Interview>> GetAllByUserIdAsync(long userId);
    }
}
