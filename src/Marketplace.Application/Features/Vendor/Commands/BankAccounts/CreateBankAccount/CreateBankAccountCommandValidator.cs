using FluentValidation;

namespace Marketplace.Application.Features.Vendor.Commands.BankAccounts.CreateBankAccount;

public class CreateBankAccountCommandValidator : AbstractValidator<CreateBankAccountCommand>
{
    public CreateBankAccountCommandValidator()
    {
        RuleFor(x => x.BankName)
            .NotEmpty().WithMessage("Bank name is required")
            .MaximumLength(100).WithMessage("Bank name must not exceed 100 characters");

        RuleFor(x => x.AccountHolderName)
            .NotEmpty().WithMessage("Account holder name is required")
            .MaximumLength(200).WithMessage("Account holder name must not exceed 200 characters");

        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Account number is required")
            .MinimumLength(4).WithMessage("Account number must be at least 4 characters")
            .MaximumLength(50).WithMessage("Account number must not exceed 50 characters");

        RuleFor(x => x.RoutingNumber)
            .NotEmpty().WithMessage("Routing number is required")
            .Matches(@"^\d{9}$").WithMessage("Routing number must be 9 digits")
            .When(x => x.Country == "US" || string.IsNullOrEmpty(x.Country));

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be a 3-letter code (e.g., USD, EUR)");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required")
            .Length(2).WithMessage("Country must be a 2-letter code (e.g., US, GB)");
    }
}
