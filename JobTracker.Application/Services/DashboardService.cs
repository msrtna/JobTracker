using JobTracker.Application.DTOs.DashboardDtos;
using JobTracker.Application.Interfaces.Repository;
using JobTracker.Application.Interfaces.Service;

namespace JobTracker.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IJobApplicationRepository _repository;
        private readonly IUserContext _userContext;

        public DashboardService(
            IJobApplicationRepository repository,
            IUserContext userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var userId = _userContext.UserId;
            return await _repository.GetDashboardAsync(userId);
        }
    }
}