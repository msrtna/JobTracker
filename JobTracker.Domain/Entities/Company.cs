using JobTracker.Domain.Common;

namespace JobTracker.Domain.Entities
{
    public class Company : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string? Location { get; set; }

        public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
    }
}
