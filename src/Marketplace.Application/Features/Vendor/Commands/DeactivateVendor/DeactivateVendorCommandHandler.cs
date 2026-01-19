using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Vendor.Commands.DeactivateVendor;

public class DeactivateVendorCommandHandler : IRequestHandler<DeactivateVendorCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeactivateVendorCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(DeactivateVendorCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<bool>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<bool>.Failure(
                "Vendor not found",
                new List<string> { "Vendor profile does not exist" }
            );
        }

        vendor.IsActive = request.IsActive;
        vendor.AcceptOrders = request.IsActive; // If deactivated, don't accept orders
        vendor.UpdatedAt = DateTime.UtcNow;
        vendor.UpdatedBy = _currentUserService.Email;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, 
            request.IsActive ? "Vendor account activated" : "Vendor account deactivated");
    }
}
