using JobTracker.Domain.Enums;

namespace JobTracker.Application.DTOs.JobApplicationDtos
{
    public class CreateJobApplicationDto
    {
        public string Location { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal? Salary { get; set; }
        public long JobCategoryId { get; set; }
        public long CompanyId { get; set; }
        public WorkPlace WorkPlace { get; set; }
        public string? JobUrl { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public JobApplicationStatus Status { get; set; }
        public string? Description { get; set; }
    }
}
