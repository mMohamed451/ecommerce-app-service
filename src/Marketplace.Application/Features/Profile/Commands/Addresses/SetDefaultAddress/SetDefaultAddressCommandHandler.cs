using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Profile.Commands.Addresses.SetDefaultAddress;

public class SetDefaultAddressCommandHandler : IRequestHandler<SetDefaultAddressCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public SetDefaultAddressCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(SetDefaultAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.UserId == userId.Value, cancellationToken);

        if (address == null)
        {
            return Result.Failure(
                "Address not found",
                new List<string> { "Address does not exist or you don't have permission to access it" }
            );
        }

        // Unset other default addresses
        var existingDefaults = await _context.Addresses
            .Where(a => a.UserId == userId.Value && a.IsDefault && a.Id != request.Id)
            .ToListAsync(cancellationToken);

        foreach (var addr in existingDefaults)
        {
            addr.IsDefault = false;
            addr.UpdatedAt = DateTime.UtcNow;
        }

        // Set this address as default
        address.IsDefault = true;
        address.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Default address updated successfully");
    }
}
