using AutoMapper;
using JobTracker.Application.DTOs.CompanyDtos;
using JobTracker.Application.Exceptions;
using JobTracker.Application.Interfaces;
using JobTracker.Domain.Entities;

namespace JobTracker.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _repository;
        private readonly IMapper _mapper;

        public CompanyService(ICompanyRepository companyRepository, IMapper mapper)
        {
            _repository = companyRepository;
            _mapper = mapper;
        }


        public async Task<List<CompanyDto>> GetAllAsync()
        {
            var result = await _repository.GetAllAsync();
            return _mapper.Map<List<CompanyDto>>(result);
        }

        public async Task<CompanyDto?> GetByIdAsync(long id)
        {
            var company = await _repository.GetByIdAsync(id);

            if (company == null)
                throw new NotFoundException($"Company with id {id} was not found.");

            return _mapper.Map<CompanyDto>(company);
        }

        public async Task<CompanyDto> CreateAsync(CreateCompanyDto dto)
        {
            if (await _repository.ExistsByNameAsync(dto.Name))
                throw new ConflictException($"Company with name '{dto.Name}' already exists.");
            if (await _repository.ExistsByWebsiteAsync(dto.Website))
                throw new ConflictException($"Company with website {dto.Website} already exists");

            var company = _mapper.Map<Company>(dto);
            await _repository.CreateAsync(company);
            return _mapper.Map<CompanyDto>(company);
        }

        public async Task UpdateAsync(long id, UpdateCompanyDto dto)
        {
            var company = await _repository.GetByIdAsync(id);

            if (company == null)
                throw new NotFoundException($"Company with id {id} was not found.");
            if (await _repository.ExistsByNameAsync(dto.Name, id))
                throw new ConflictException($"Company with name '{dto.Name}' already exists.");
            if (await _repository.ExistsByWebsiteAsync(dto.Website, id))
                throw new ConflictException($"Company with website {dto.Website} already exists");

            _mapper.Map(dto, company);
            await _repository.UpdateAsync(company);
        }

        public async Task DeleteAsync(long id)
        {
            var company = await _repository.GetByIdAsync(id);
            if (company == null)
                throw new NotFoundException($"Company with id {id} was not found.");

            await _repository.DeleteAsync(company);
        }
    }
}
