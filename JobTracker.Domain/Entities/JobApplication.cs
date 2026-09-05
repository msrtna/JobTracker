using JobTracker.Domain.Common;
using JobTracker.Domain.Enums;

namespace JobTracker.Domain.Entities
{
    public class JobApplication :BaseEntity
    {
        public string Location { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal? Salary { get; set; }
        public long JobCategoryId { get; set; }
        public long CompanyId { get; set; }
        public long UserId { get; set; }
        public WorkPlace WorkPlace { get; set; }
        public string? JobUrl { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public JobApplicationStatus Status { get; set; }
        public string? Description { get; set; }

        public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
        public User User { get; set; } = null!;
        public Company Company { get; set; } = null!;
        public JobCategory JobCategory { get; set; } = null!;
    }
}