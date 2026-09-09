using FluentValidation;
using JobTracker.Application.DTOs.InterviewDtos;

namespace JobTracker.Application.Validators.InterviewValidators
{
    public class CreateInterviewDtoValidator :AbstractValidator<CreateInterviewDto>
    {
        public CreateInterviewDtoValidator()
        {
            RuleFor(x => x.JobApplicationId)
                .NotEmpty()
                .WithMessage("Job application id is required.");

            RuleFor(x => x.InterviewDate)
                .NotEmpty()
                .WithMessage("Interview date is required.");

            RuleFor(x => x.InterviewType)
                .IsInEnum()
                .WithMessage("Invalid interview type.");

            RuleFor(x => x.Notes)
                .MaximumLength(200)
                .WithMessage("Company location cannot exceed 200 characters.");
        }
    }
}
