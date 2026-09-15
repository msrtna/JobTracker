using JobTracker.Domain.Enums;

namespace JobTracker.Application.DTOs.JobApplicationDtos
{
    public class JobApplicationQueryDto
    {
        // Search
        public string? Search { get; set; }

        // Filters
        public JobApplicationStatus? Status { get; set; }
        public WorkPlace? WorkPlace { get; set; }
        public long? CompanyId { get; set; }
        public long? JobCategoryId { get; set; }

        public DateTime? ApplicationDateFrom { get; set; }
        public DateTime? ApplicationDateTo { get; set; }

        // Sorting
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}