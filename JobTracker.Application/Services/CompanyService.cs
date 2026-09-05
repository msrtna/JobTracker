using JobTracker.Application.DTOs.CompanyDtos;
using JobTracker.Application.Interfaces;

namespace JobTracker.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }


        public Task<List<CompanyDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CompanyDto?> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<CompanyDto> CreateAsync(CreateCompanyDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(long id, UpdateCompanyDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }
    }
}
