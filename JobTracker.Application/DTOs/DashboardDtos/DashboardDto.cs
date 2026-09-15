namespace JobTracker.Application.DTOs.DashboardDtos
{
    public class DashboardDto
    {
        public int TotalApplications { get; set; }

        public int Saved { get; set; }
        public int Applied { get; set; }
        public int Interviews { get; set; }
        public int Offers { get; set; }
        public int Rejected { get; set; }
        public int Withdrawn { get; set; }

        public int Remote { get; set; }
        public int Hybrid { get; set; }
        public int OnSite { get; set; }
    }
}