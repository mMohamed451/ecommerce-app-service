using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Vendor.Commands.TaxInfo.CreateTaxInfo;

public class CreateTaxInfoCommandHandler : IRequestHandler<CreateTaxInfoCommand, Result<TaxInfoResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateTaxInfoCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<TaxInfoResponse>> Handle(CreateTaxInfoCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<TaxInfoResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<TaxInfoResponse>.Failure(
                "Vendor not found",
                new List<string> { "Vendor account does not exist" }
            );
        }

        // Check if tax ID already exists for another vendor
        var existingTaxId = await _context.VendorTaxInfos
            .FirstOrDefaultAsync(t => t.TaxId == request.TaxId && t.VendorId != vendor.Id, cancellationToken);

        if (existingTaxId != null)
        {
            return Result<TaxInfoResponse>.Failure(
                "Tax ID already exists",
                new List<string> { "This tax ID is already registered to another vendor" }
            );
        }

        var taxInfo = new VendorTaxInfo
        {
            VendorId = vendor.Id,
            TaxId = request.TaxId, // In production, consider encrypting this
            TaxIdType = request.TaxIdType,
            Country = request.Country,
            State = request.State,
            TaxRate = request.TaxRate,
            CollectsTax = request.CollectsTax,
            IsVerified = false,
            CreatedBy = userId.Value.ToString()
        };

        _context.VendorTaxInfos.Add(taxInfo);
        await _context.SaveChangesAsync(cancellationToken);

        // Mask tax ID for response
        var maskedTaxId = MaskTaxId(request.TaxId);

        return Result<TaxInfoResponse>.Success(new TaxInfoResponse
        {
            Id = taxInfo.Id,
            MaskedTaxId = maskedTaxId,
            TaxIdType = taxInfo.TaxIdType,
            Country = taxInfo.Country,
            State = taxInfo.State,
            IsVerified = taxInfo.IsVerified,
            CollectsTax = taxInfo.CollectsTax
        });
    }

    private static string MaskTaxId(string taxId)
    {
        if (string.IsNullOrEmpty(taxId) || taxId.Length < 4)
            return "****";

        return "****" + taxId.Substring(taxId.Length - 4);
    }
}
