using FluentValidation;

namespace Marketplace.Application.Features.Vendor.Commands.RegisterVendor;

public class RegisterVendorCommandValidator : AbstractValidator<RegisterVendorCommand>
{
    public RegisterVendorCommandValidator()
    {
        RuleFor(x => x.BusinessName)
            .NotEmpty().WithMessage("Business name is required")
            .MinimumLength(2).WithMessage("Business name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Business name must not exceed 100 characters");

        RuleFor(x => x.BusinessEmail)
            .NotEmpty().WithMessage("Business email is required")
            .EmailAddress().WithMessage("Invalid email address");

        RuleFor(x => x.BusinessPhone)
            .NotEmpty().WithMessage("Business phone is required")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format");

        RuleFor(x => x.Website)
            .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Invalid website URL")
            .When(x => !string.IsNullOrEmpty(x.Website));

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street address is required")
            .MinimumLength(5).WithMessage("Street address must be at least 5 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MinimumLength(2).WithMessage("City must be at least 2 characters");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .MinimumLength(2).WithMessage("State must be at least 2 characters");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("ZIP code is required")
            .Matches(@"^\d{5}(-\d{4})?$").WithMessage("Invalid ZIP code format");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required")
            .MinimumLength(2).WithMessage("Country must be at least 2 characters");

        RuleFor(x => x.Documents)
            .NotEmpty().WithMessage("At least one verification document is required")
            .Must(docs => docs != null && docs.Count > 0).WithMessage("At least one document is required");
    }
}
