using System.ComponentModel.DataAnnotations;

namespace JobTracker.Application.DTOs.ComponyDtos
{
    public class CreateCompanyDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }
    }
}
