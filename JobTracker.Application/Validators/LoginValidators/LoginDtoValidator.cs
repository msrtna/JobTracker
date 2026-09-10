using FluentValidation;
using JobTracker.Application.DTOs.AuthDtos.LoginDtos;

namespace JobTracker.Application.Validators.LoginValidators
{
    public class LoginDtoValidator :AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                    .NotEmpty()
                    .WithMessage("Email is required.")
                    .EmailAddress()
                    .WithMessage("Invalid email address.");

            RuleFor(x => x.Password)
                    .NotEmpty()
                    .WithMessage("Password is required.");
        }
    }
}
