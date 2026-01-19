using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Vendor.Commands.Commissions.CreateCommission;

public class CreateCommissionCommandHandler : IRequestHandler<CreateCommissionCommand, Result<CommissionResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateCommissionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CommissionResponse>> Handle(CreateCommissionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<CommissionResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<CommissionResponse>.Failure(
                "Vendor not found",
                new List<string> { "Vendor account does not exist" }
            );
        }

        // Deactivate existing active commissions
        var existingActive = await _context.VendorCommissions
            .Where(c => c.VendorId == vendor.Id && c.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var existingCommission in existingActive)
        {
            existingCommission.IsActive = false;
        }

        var newCommission = new VendorCommission
        {
            VendorId = vendor.Id,
            CommissionPercentage = request.CommissionPercentage,
            FixedFee = request.FixedFee,
            MinimumCommission = request.MinimumCommission,
            MaximumCommission = request.MaximumCommission,
            CommissionType = request.CommissionType,
            IsActive = true,
            EffectiveFrom = request.EffectiveFrom ?? DateTime.UtcNow,
            EffectiveTo = request.EffectiveTo,
            Notes = request.Notes,
            CreatedBy = userId.Value.ToString()
        };

        _context.VendorCommissions.Add(newCommission);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<CommissionResponse>.Success(new CommissionResponse
        {
            Id = newCommission.Id,
            CommissionPercentage = newCommission.CommissionPercentage,
            FixedFee = newCommission.FixedFee,
            CommissionType = newCommission.CommissionType,
            IsActive = newCommission.IsActive,
            EffectiveFrom = newCommission.EffectiveFrom,
            EffectiveTo = newCommission.EffectiveTo
        });
    }
}
