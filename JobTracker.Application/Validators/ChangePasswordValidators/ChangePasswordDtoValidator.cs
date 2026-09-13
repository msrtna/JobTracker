using FluentValidation;
using JobTracker.Application.DTOs.AuthDtos.ChangePasswordDtos;

namespace JobTracker.Application.Validators.ChangePasswordValidators
{
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage("Current password is required");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("New password is required")
                .MinimumLength(8)
                .WithMessage("New password must be at least 8 characters");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty()
                .WithMessage("Confirm new password is required")
                .Equal(x => x.NewPassword)
                .WithMessage("New password and confirmation do not match");
        }
    }
}
