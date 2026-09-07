using JobTracker.Application.DTOs.InterviewDtos;

namespace JobTracker.Application.Interfaces
{
    public interface IInterviewService
    {
        Task<List<InterviewDto>> GetAllAsync();
        Task<InterviewDto?> GetByIdAsync(long id);
        Task<InterviewDto> CreateAsync(CreateInterviewDto dto);
        Task UpdateAsync(long id, UpdateInterviewDto dto);
        Task DeleteAsync(long id);
    }
}
