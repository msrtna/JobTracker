namespace JobTracker.Application.DTOs.JobCategoryDtos
{
    public class JobCategoryDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}