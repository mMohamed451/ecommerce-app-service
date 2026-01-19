using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Commands.BankAccounts.CreateBankAccount;

public class CreateBankAccountCommand : IRequest<Result<BankAccountResponse>>
{
    public string BankName { get; set; } = string.Empty;
    public string AccountHolderName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string RoutingNumber { get; set; } = string.Empty;
    public string? SwiftCode { get; set; }
    public string? Iban { get; set; }
    public string Currency { get; set; } = "USD";
    public string Country { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

public class BankAccountResponse
{
    public Guid Id { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string AccountHolderName { get; set; } = string.Empty;
    public string MaskedAccountNumber { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsVerified { get; set; }
}
