using JobTracker.Application.DTOs.JobCategoryDtos;

namespace JobTracker.Application.Interfaces.Service
{
    public interface IJobCategoryService
    {
        Task<List<JobCategoryDto>> GetAllAsync();
        Task<JobCategoryDto?> GetByIdAsync(long id);
        Task<JobCategoryDto> CreateAsync(CreateJobCategoryDto dto);
        Task UpdateAsync(long id, UpdateJobCategoryDto dto);
        Task DeleteAsync(long id);
    }
}
