using FluentValidation;
using JobTracker.Application.DTOs.AuthDtos.RegisterDtos;

namespace JobTracker.Application.Validators.RegisterValidator
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.FirstName)
                    .NotEmpty()
                    .WithMessage("First name is required.")
                    .MaximumLength(100)
                    .WithMessage("First name cannot exceed 100 characters.");

            RuleFor(x => x.LastName)
                    .NotEmpty()
                    .WithMessage("Last name is required.")
                    .MaximumLength(100)
                    .WithMessage("Last name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                    .NotEmpty()
                    .WithMessage("Email is required.")
                    .MaximumLength(100)
                    .WithMessage("Email cannot exceed 100 characters.")
                    .EmailAddress()
                    .WithMessage("Invalid email address."); ;

            RuleFor(x => x.Password)
                    .NotEmpty()
                    .WithMessage("Password is required.")
                    .MaximumLength(100)
                    .WithMessage("Password cannot exceed 100 characters.")
                    .MinimumLength(8)
                    .WithMessage("Password must be at least 8 characters long."); ;
        }
    }
}
