using JobTracker.Domain.Common;
using JobTracker.Domain.Enums;

namespace JobTracker.Domain.Entities
{
    public class Interview : BaseEntity
    {
        public long JobApplicationId { get; set; }
        public DateTime InterviewDate { get; set; }
        public InterviewType InterviewType { get; set; }
        public string? Notes { get; set; }

        public JobApplication JobApplication { get; set; } = null!;
    }
}
