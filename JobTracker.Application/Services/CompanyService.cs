using AutoMapper;
using JobTracker.Application.DTOs.CompanyDtos;
using JobTracker.Application.Interfaces;
using JobTracker.Domain.Entities;

namespace JobTracker.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompanyService(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }


        public async Task<List<CompanyDto>> GetAllAsync()
        {
            var result = await _companyRepository.GetAllAsync();
            return _mapper.Map<List<CompanyDto>>(result);
        }

        public async Task<CompanyDto?> GetByIdAsync(long id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            return _mapper.Map<CompanyDto>(company);
        }

        public async Task<CompanyDto> CreateAsync(CreateCompanyDto dto)
        {
            var company = _mapper.Map<Company>(dto);
            await _companyRepository.CreateAsync(company);
            return _mapper.Map<CompanyDto>(company);
        }

        public async Task<bool> UpdateAsync(long id, UpdateCompanyDto dto)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null)
                return false;

            _mapper.Map(dto, company);

            await _companyRepository.UpdateAsync(company);
            return true;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null)
                return false;

            await _companyRepository.DeleteAsync(company);
            return true;
        }
    }
}
