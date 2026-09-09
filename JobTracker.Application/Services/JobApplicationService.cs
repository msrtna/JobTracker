using AutoMapper;
using JobTracker.Application.DTOs.InterviewDtos;
using JobTracker.Application.DTOs.JobApplicationDtos;
using JobTracker.Application.Exceptions;
using JobTracker.Application.Interfaces;
using JobTracker.Domain.Entities;

namespace JobTracker.Application.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;
        private readonly IJobCategoryRepository _jobCategoryRepository;

        public JobApplicationService(
                IJobApplicationRepository repository,
                IMapper mapper,
                ICompanyRepository companyRepository,
                IJobCategoryRepository jobCategoryRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _companyRepository = companyRepository;
            _jobCategoryRepository = jobCategoryRepository;
        }


        public async Task<List<JobApplicationDto>> GetAllAsync()
        {
            var result = await _repository.GetAllAsync();
            return _mapper.Map<List<JobApplicationDto>>(result);
        }

        public async Task<JobApplicationDto?> GetByIdAsync(long id)
        {
            var result = await _repository.GetByIdAsync(id);
            if (result == null)
                throw new NotFoundException($"Job application with id {id} was not found.");
            return _mapper.Map<JobApplicationDto>(result);
        }

        public async Task<JobApplicationDto> CreateAsync(CreateJobApplicationDto dto)
        {
            if (!await _companyRepository.ExistsAsync(dto.CompanyId))
                throw new NotFoundException($"Company with id {dto.CompanyId} was not found.");
            if (!await _jobCategoryRepository.ExistsAsync(dto.JobCategoryId))
                throw new NotFoundException($"Job category with id {dto.JobCategoryId} was not found.");

            var result = _mapper.Map<JobApplication>(dto);
            await _repository.CreateAsync(result);
            return _mapper.Map<JobApplicationDto>(result);
        }

        public async Task UpdateAsync(long id, UpdateJobApplicationDto dto)
        {
            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                throw new NotFoundException($"Job application with id {id} was not found.");
            if (!await _companyRepository.ExistsAsync(dto.CompanyId))
                throw new NotFoundException($"Company with id {dto.CompanyId} was not found.");
            if (!await _jobCategoryRepository.ExistsAsync(dto.JobCategoryId))
                throw new NotFoundException($"Job category with id {dto.JobCategoryId} was not found.");

            _mapper.Map(dto, result);
            await _repository.UpdateAsync(result);
        }

        public async Task DeleteAsync(long id)
        {
            var result = await _repository.GetByIdAsync(id);
            if (result == null)
                throw new NotFoundException($"Job application with id {id} was not found.");

            await _repository.DeleteAsync(result);
        }
    }
}
