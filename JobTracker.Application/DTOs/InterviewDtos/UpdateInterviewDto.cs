using JobTracker.Domain.Enums;

namespace JobTracker.Application.DTOs.InterviewDtos
{
    public class UpdateInterviewDto
    {
        public long JobApplicationId { get; set; }
        public DateTime InterviewDate { get; set; }
        public InterviewType InterviewType { get; set; }
        public string? Notes { get; set; }
    }
}
