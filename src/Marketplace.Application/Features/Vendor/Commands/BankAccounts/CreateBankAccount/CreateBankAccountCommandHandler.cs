using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Vendor.Commands.BankAccounts.CreateBankAccount;

public class CreateBankAccountCommandHandler : IRequestHandler<CreateBankAccountCommand, Result<BankAccountResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateBankAccountCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BankAccountResponse>> Handle(CreateBankAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<BankAccountResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<BankAccountResponse>.Failure(
                "Vendor not found",
                new List<string> { "Vendor account does not exist" }
            );
        }

        // If this is set as default, unset other default accounts
        if (request.IsDefault)
        {
            var existingDefaults = await _context.VendorBankAccounts
                .Where(ba => ba.VendorId == vendor.Id && ba.IsDefault)
                .ToListAsync(cancellationToken);

            foreach (var account in existingDefaults)
            {
                account.IsDefault = false;
            }
        }

        var bankAccount = new VendorBankAccount
        {
            VendorId = vendor.Id,
            BankName = request.BankName,
            AccountHolderName = request.AccountHolderName,
            AccountNumber = request.AccountNumber, // In production, encrypt this
            RoutingNumber = request.RoutingNumber, // In production, encrypt this
            SwiftCode = request.SwiftCode,
            Iban = request.Iban,
            Currency = request.Currency,
            Country = request.Country,
            IsDefault = request.IsDefault,
            IsVerified = false,
            CreatedBy = userId.Value.ToString()
        };

        _context.VendorBankAccounts.Add(bankAccount);
        await _context.SaveChangesAsync(cancellationToken);

        // Mask account number for response
        var maskedAccountNumber = MaskAccountNumber(request.AccountNumber);

        return Result<BankAccountResponse>.Success(new BankAccountResponse
        {
            Id = bankAccount.Id,
            BankName = bankAccount.BankName,
            AccountHolderName = bankAccount.AccountHolderName,
            MaskedAccountNumber = maskedAccountNumber,
            IsDefault = bankAccount.IsDefault,
            IsVerified = bankAccount.IsVerified
        });
    }

    private static string MaskAccountNumber(string accountNumber)
    {
        if (string.IsNullOrEmpty(accountNumber) || accountNumber.Length < 4)
            return "****";

        return "****" + accountNumber.Substring(accountNumber.Length - 4);
    }
}
