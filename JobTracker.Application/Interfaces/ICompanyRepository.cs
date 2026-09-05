using JobTracker.Domain.Entities;

namespace JobTracker.Application.Interfaces
{
    public interface ICompanyRepository
    {
        Task<List<Company>> GetAllAsync();
        Task<Company?> GetByIdAsync(long id);
        Task CreateAsync(Company company);
        Task UpdateAsync(Company company);
        Task DeleteAsync(Company company);
    }
}
