namespace JobTracker.Application.DTOs.CompanyDtos
{
    public class CreateCompanyDto
    {
        public string Name { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string? Location { get; set; }
    }
}
