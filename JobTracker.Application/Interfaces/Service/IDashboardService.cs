using JobTracker.Application.DTOs.DashboardDtos;

namespace JobTracker.Application.Interfaces.Service
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardAsync();
    }
}