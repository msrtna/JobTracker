using AutoMapper;
using JobTracker.Application.DTOs.JobCategoryDtos;
using JobTracker.Application.Exceptions;
using JobTracker.Application.Interfaces;
using JobTracker.Domain.Entities;

namespace JobTracker.Application.Services
{
    public class JobCategoryService : IJobCategoryService
    {
        private readonly IJobCategoryRepository _repository;
        private readonly IMapper _mapper;

        public JobCategoryService(IJobCategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        public async Task<List<JobCategoryDto>> GetAllAsync()
        {
            var result = await _repository.GetAllAsync();
            return _mapper.Map<List<JobCategoryDto>>(result);
        }

        public async Task<JobCategoryDto?> GetByIdAsync(long id)
        {
            var result = await _repository.GetByIdAsync(id);
            if (result == null)
                throw new NotFoundException($"JobCategory with id {id} was not found.");
            return _mapper.Map<JobCategoryDto>(result);
        }

        public async Task<JobCategoryDto> CreateAsync(CreateJobCategoryDto dto)
        {
            if (await _repository.ExistsByNameAsync(dto.Name))
                throw new ConflictException($"JobCategory with name '{dto.Name}' already exists.");

            var result = _mapper.Map<JobCategory>(dto);
            await _repository.CreateAsync(result);
            return _mapper.Map<JobCategoryDto>(result);
        }

        public async Task UpdateAsync(long id, UpdateJobCategoryDto dto)
        {
            var result = await _repository.GetByIdAsync(id);

            if (result == null)
                throw new NotFoundException($"JobCategory with id {id} was not found.");
            if (await _repository.ExistsByNameAsync(dto.Name, id))
                throw new ConflictException($"JobCategory with name '{dto.Name}' already exists.");

            _mapper.Map(dto, result);
            await _repository.UpdateAsync(result);
        }

        public async Task DeleteAsync(long id)
        {
            var result = await _repository.GetByIdAsync(id);
            if (result == null)
                throw new NotFoundException($"JobCategory with id {id} was not found.");

            await _repository.DeleteAsync(result);
        }
    }
}
