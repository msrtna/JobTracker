using JobTracker.Application.DTOs.CompanyDtos;

namespace JobTracker.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<List<CompanyDto>> GetAllAsync();
        Task<CompanyDto?> GetByIdAsync(long id);
        Task<CompanyDto> CreateAsync(CreateCompanyDto dto);
        Task UpdateAsync(long id, UpdateCompanyDto dto);
        Task DeleteAsync(long id);
    }
}
