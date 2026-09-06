using FluentValidation;
using JobTracker.Application.DTOs.JobCategoryDtos;

namespace JobTracker.Application.Validators.JobCategoryValidators
{
    public class CreateJobCategoryDtoValidator : AbstractValidator<CreateJobCategoryDto>
    {
        public CreateJobCategoryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("JobCategory name is required.")
                .MaximumLength(100)
                .WithMessage("JobCategory name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("JobCategory Description cannot exceed 500 characters.");
        }
    }
}
