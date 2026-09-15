using JobTracker.Application.DTOs.Common;
using JobTracker.Application.DTOs.JobApplicationDtos;

namespace JobTracker.Application.Interfaces.Service
{
    public interface IJobApplicationService
    {
        Task<PagedResultDto<JobApplicationDto>> GetAllAsync(JobApplicationQueryDto query);
        Task<JobApplicationDto?> GetByIdAsync(long id);
        Task<JobApplicationDto> CreateAsync(CreateJobApplicationDto dto);
        Task UpdateAsync(long id, UpdateJobApplicationDto dto);
        Task DeleteAsync(long id);
    }
}
