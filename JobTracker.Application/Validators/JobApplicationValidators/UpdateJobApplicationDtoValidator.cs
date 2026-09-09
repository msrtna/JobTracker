using FluentValidation;
using JobTracker.Application.DTOs.JobApplicationDtos;

namespace JobTracker.Application.Validators.JobApplicationValidators
{
    public class UpdateJobApplicationDtoValidator : AbstractValidator<UpdateJobApplicationDto>
    {
        public UpdateJobApplicationDtoValidator()
        {
            RuleFor(x => x.Location)
                    .NotEmpty()
                    .WithMessage("Job Application location is required.")
                    .MaximumLength(100)
                    .WithMessage("Job Application location cannot exceed 100 characters.");

            RuleFor(x => x.Position)
                    .NotEmpty()
                    .WithMessage("Job Application position is required.")
                    .MaximumLength(100)
                    .WithMessage("Job Application position cannot exceed 100 characters.");

            RuleFor(x => x.WorkPlace)
                    .IsInEnum()
                    .WithMessage("Invalid Job Application work place.");

            RuleFor(x => x.Status)
                    .IsInEnum()
                    .WithMessage("Invalid Job Application status.");

            RuleFor(x => x.JobUrl)
                    .MaximumLength(200)
                    .WithMessage("Job Application Url cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                    .MaximumLength(500)
                    .WithMessage("Job Application description cannot exceed 500 characters.");

            RuleFor(x => x.JobCategoryId)
                    .NotEmpty()
                    .WithMessage("Job category id is required.");

            RuleFor(x => x.CompanyId)
                    .NotEmpty()
                    .WithMessage("Company id is required.");

            RuleFor(x => x.UserId)
                    .NotEmpty()
                    .WithMessage("User id is required.");
        }
    }
}
