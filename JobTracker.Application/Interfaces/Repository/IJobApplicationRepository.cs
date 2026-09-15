using JobTracker.Application.DTOs.Common;
using JobTracker.Application.DTOs.DashboardDtos;
using JobTracker.Application.DTOs.JobApplicationDtos;
using JobTracker.Domain.Entities;

namespace JobTracker.Application.Interfaces.Repository
{
    public interface IJobApplicationRepository
    {
        Task<List<JobApplication>> GetAllAsync();
        Task<JobApplication?> GetByIdAsync(long id);
        Task CreateAsync(JobApplication jobApplication);
        Task UpdateAsync(JobApplication jobApplication);
        Task DeleteAsync(JobApplication jobApplication);
        Task<bool> ExistsAsync(long id);
        Task<PagedResultDto<JobApplication>> GetAllByUserIdAsync(long userId, JobApplicationQueryDto query);
        Task<DashboardDto> GetDashboardAsync(long userId);
    }
}
