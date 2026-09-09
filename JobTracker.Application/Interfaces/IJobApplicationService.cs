using JobTracker.Application.DTOs.JobApplicationDtos;

namespace JobTracker.Application.Interfaces
{
    public interface IJobApplicationService
    {
        Task<List<JobApplicationDto>> GetAllAsync();
        Task<JobApplicationDto?> GetByIdAsync(long id);
        Task<JobApplicationDto> CreateAsync(CreateJobApplicationDto dto);
        Task UpdateAsync(long id, UpdateJobApplicationDto dto);
        Task DeleteAsync(long id);
    }
}
