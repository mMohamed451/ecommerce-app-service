using FluentValidation;

namespace Marketplace.Application.Features.Vendor.Commands.UpdateVendorProfile;

public class UpdateVendorProfileCommandValidator : AbstractValidator<UpdateVendorProfileCommand>
{
    public UpdateVendorProfileCommandValidator()
    {
        RuleFor(x => x.BusinessName)
            .MinimumLength(2).WithMessage("Business name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Business name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.BusinessName));

        RuleFor(x => x.BusinessEmail)
            .EmailAddress().WithMessage("Invalid email address")
            .When(x => !string.IsNullOrEmpty(x.BusinessEmail));

        RuleFor(x => x.BusinessPhone)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrEmpty(x.BusinessPhone));

        RuleFor(x => x.Website)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Invalid website URL")
            .When(x => !string.IsNullOrEmpty(x.Website));

        RuleFor(x => x.ZipCode)
            .Matches(@"^\d{5}(-\d{4})?$").WithMessage("Invalid ZIP code format")
            .When(x => !string.IsNullOrEmpty(x.ZipCode));
    }
}
