using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Profile.Commands.Addresses.DeleteAddress;

public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteAddressCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
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
                new List<string> { "Address does not exist or you don't have permission to delete it" }
            );
        }

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Address deleted successfully");
    }
}
