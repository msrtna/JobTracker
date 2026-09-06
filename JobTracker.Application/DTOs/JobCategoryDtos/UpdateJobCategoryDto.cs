namespace JobTracker.Application.DTOs.JobCategoryDtos
{
    public class UpdateJobCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}