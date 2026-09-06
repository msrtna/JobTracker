namespace JobTracker.Application.DTOs.JobCategoryDtos
{
    public class CreateJobCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}