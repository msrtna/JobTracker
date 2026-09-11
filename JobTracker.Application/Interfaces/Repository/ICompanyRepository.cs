using JobTracker.Domain.Entities;

namespace JobTracker.Application.Interfaces.Repository
{
    public interface ICompanyRepository
    {
        Task<List<Company>> GetAllAsync();
        Task<Company?> GetByIdAsync(long id);
        Task CreateAsync(Company company);
        Task UpdateAsync(Company company);
        Task DeleteAsync(Company company);
        Task<bool> ExistsAsync(long id);
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<bool> ExistsByWebsiteAsync(string website, long? excludeId = null);
    }
}
