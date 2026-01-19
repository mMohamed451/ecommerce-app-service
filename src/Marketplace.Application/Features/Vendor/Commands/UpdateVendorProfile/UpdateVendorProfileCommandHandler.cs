using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Vendor.DTOs;
using Marketplace.Application.Features.Vendor.Queries.GetVendorProfile;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Vendor.Commands.UpdateVendorProfile;

public class UpdateVendorProfileCommandHandler : IRequestHandler<UpdateVendorProfileCommand, Result<VendorDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public UpdateVendorProfileCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IMediator mediator)
    {
        _context = context;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<Result<VendorDto>> Handle(UpdateVendorProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<VendorDto>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<VendorDto>.Failure(
                "Vendor not found",
                new List<string> { "Vendor profile does not exist" }
            );
        }

        // Check if business email is already taken by another vendor
        if (!string.IsNullOrEmpty(request.BusinessEmail) && request.BusinessEmail != vendor.BusinessEmail)
        {
            var emailExists = await _context.Vendors
                .AnyAsync(v => v.BusinessEmail == request.BusinessEmail && v.Id != vendor.Id, cancellationToken);
            
            if (emailExists)
            {
                return Result<VendorDto>.Failure(
                    "Email already registered",
                    new List<string> { "This business email is already registered" }
                );
            }
        }

        // Update fields if provided
        if (!string.IsNullOrEmpty(request.BusinessName))
            vendor.BusinessName = request.BusinessName;
        
        if (request.BusinessDescription != null)
            vendor.BusinessDescription = request.BusinessDescription;
        
        if (!string.IsNullOrEmpty(request.BusinessEmail))
            vendor.BusinessEmail = request.BusinessEmail;
        
        if (!string.IsNullOrEmpty(request.BusinessPhone))
            vendor.BusinessPhone = request.BusinessPhone;
        
        if (request.Website != null)
            vendor.Website = request.Website;
        
        if (!string.IsNullOrEmpty(request.Street))
            vendor.Street = request.Street;
        
        if (!string.IsNullOrEmpty(request.City))
            vendor.City = request.City;
        
        if (!string.IsNullOrEmpty(request.State))
            vendor.State = request.State;
        
        if (!string.IsNullOrEmpty(request.ZipCode))
            vendor.ZipCode = request.ZipCode;
        
        if (!string.IsNullOrEmpty(request.Country))
            vendor.Country = request.Country;
        
        if (request.TaxId != null)
            vendor.TaxId = request.TaxId;
        
        if (request.RegistrationNumber != null)
            vendor.RegistrationNumber = request.RegistrationNumber;

        vendor.UpdatedAt = DateTime.UtcNow;
        vendor.UpdatedBy = _currentUserService.Email;

        await _context.SaveChangesAsync(cancellationToken);

        // Return updated vendor profile
        return await _mediator.Send(new GetVendorProfileQuery(), cancellationToken);
    }
}
