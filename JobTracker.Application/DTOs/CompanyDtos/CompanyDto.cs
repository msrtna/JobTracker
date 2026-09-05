namespace JobTracker.Application.DTOs.CompanyDtos
{
    public class CompanyDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string? Location { get; set; }
    }
}
