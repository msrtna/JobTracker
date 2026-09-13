using FluentValidation;
using JobTracker.Application.DTOs.AuthDtos.RefreshTokenDtos;

namespace JobTracker.Application.Validators.RefreshTokenRequestValidators
{
    public class RefreshTokenRequestValidator
        : AbstractValidator<RefreshTokenRequestDto>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token is required.");
        }
    }
}
