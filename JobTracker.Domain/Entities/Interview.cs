using JobTracker.Domain.Enums;

namespace JobTracker.Domain.Entities
{
    public class Interview : BaseEntity
    {
        public int JobApplicationId { get; set; }
        public DateTime InterviewDate { get; set; }
        public InterviewType Type { get; set; }
        public string? Notes { get; set; }
    }
}
