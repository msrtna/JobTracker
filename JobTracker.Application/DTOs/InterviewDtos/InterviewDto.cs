using JobTracker.Domain.Enums;

namespace JobTracker.Application.DTOs.InterviewDtos
{
    public class InterviewDto
    {
        public long Id { get; set; }
        public long JobApplicationId { get; set; }
        public DateTime InterviewDate { get; set; }
        public InterviewType InterviewType { get; set; }
        public string? Notes { get; set; }
    }
}
