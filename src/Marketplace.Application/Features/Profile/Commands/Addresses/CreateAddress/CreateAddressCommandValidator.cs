using FluentValidation;

namespace Marketplace.Application.Features.Profile.Commands.Addresses.CreateAddress;

public class CreateAddressCommandValidator : AbstractValidator<CreateAddressCommand>
{
    public CreateAddressCommandValidator()
    {
        RuleFor(x => x.Label)
            .NotEmpty().WithMessage("Address label is required")
            .MaximumLength(50).WithMessage("Label must not exceed 50 characters");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street address is required")
            .MinimumLength(5).WithMessage("Street address must be at least 5 characters")
            .MaximumLength(200).WithMessage("Street address must not exceed 200 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .MaximumLength(100).WithMessage("State must not exceed 100 characters");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("ZIP code is required")
            .Matches(@"^\d{5}(-\d{4})?$").WithMessage("Invalid ZIP code format");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required")
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters");
    }
}
