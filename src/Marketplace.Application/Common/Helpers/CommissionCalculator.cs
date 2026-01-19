using Marketplace.Application.Common.Interfaces;
using Marketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Common.Helpers;

public class CommissionCalculator
{
    private readonly IApplicationDbContext _context;

    public CommissionCalculator(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> CalculateCommissionAsync(Guid vendorId, decimal orderAmount, CancellationToken cancellationToken = default)
    {
        // Get active commission for vendor
        var commission = await _context.VendorCommissions
            .Where(c => 
                c.VendorId == vendorId && 
                c.IsActive &&
                (c.EffectiveFrom == null || c.EffectiveFrom <= DateTime.UtcNow) &&
                (c.EffectiveTo == null || c.EffectiveTo >= DateTime.UtcNow))
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (commission == null)
        {
            // Default commission if none configured
            return orderAmount * 0.10m; // 10% default
        }

        decimal calculatedCommission = 0;

        switch (commission.CommissionType)
        {
            case CommissionType.Percentage:
                calculatedCommission = orderAmount * (commission.CommissionPercentage / 100m);
                break;

            case CommissionType.Fixed:
                calculatedCommission = commission.FixedFee ?? 0;
                break;

            case CommissionType.Hybrid:
                var percentageAmount = orderAmount * (commission.CommissionPercentage / 100m);
                var fixedAmount = commission.FixedFee ?? 0;
                calculatedCommission = percentageAmount + fixedAmount;
                break;
        }

        // Apply minimum commission if set
        if (commission.MinimumCommission.HasValue && calculatedCommission < commission.MinimumCommission.Value)
        {
            calculatedCommission = commission.MinimumCommission.Value;
        }

        // Apply maximum commission if set
        if (commission.MaximumCommission.HasValue && calculatedCommission > commission.MaximumCommission.Value)
        {
            calculatedCommission = commission.MaximumCommission.Value;
        }

        return calculatedCommission;
    }
}
