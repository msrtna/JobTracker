using FluentValidation;
using JobTracker.Application.DTOs.CompanyDtos;

namespace JobTracker.Application.Validators.CompanyValidators
{
    public class UpdateCompanyDtoValidator : AbstractValidator<UpdateCompanyDto>
    {
        public UpdateCompanyDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Company name is required.")
                .MaximumLength(100)
                .WithMessage("Company name cannot exceed 100 characters.");

            RuleFor(x => x.Website)
                .NotEmpty()
                .WithMessage("Company website is required.")
                .MaximumLength(500)
                .WithMessage("Company website cannot exceed 500 characters.")
                .Must(BeValidUrl)
                .WithMessage("Company website must be a valid URL.");

            RuleFor(x => x.Location)
                .MaximumLength(200)
                .WithMessage("Company location cannot exceed 200 characters.");
        }


        // for website validate
        private bool BeValidUrl(string website)
        {
            return Uri.TryCreate(website, UriKind.Absolute, out var uri)
                    && (uri.Scheme == Uri.UriSchemeHttp ||
                        uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
