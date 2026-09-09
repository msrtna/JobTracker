using JobTracker.Domain.Entities;

namespace JobTracker.Application.Interfaces
{
    public interface IJobCategoryRepository
    {
        Task<List<JobCategory>> GetAllAsync();
        Task<JobCategory?> GetByIdAsync(long id);
        Task CreateAsync(JobCategory jobCategory);
        Task UpdateAsync(JobCategory jobCategory);
        Task DeleteAsync(JobCategory jobCategory);
        Task<bool> ExistsAsync(long id);
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
    }
}
