using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Vendor.DTOs;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Vendor.Commands.SuspendVendor;

public class SuspendVendorCommandHandler : IRequestHandler<SuspendVendorCommand, Result<VendorDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;

    public SuspendVendorCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<Result<VendorDto>> Handle(SuspendVendorCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<VendorDto>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var adminUser = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (adminUser == null)
        {
            return Result<VendorDto>.Failure(
                "User not found",
                new List<string> { "User does not exist" }
            );
        }

        var isAdmin = await _userManager.IsInRoleAsync(adminUser, "Admin");
        if (!isAdmin)
        {
            return Result<VendorDto>.Failure(
                "Forbidden",
                new List<string> { "Only administrators can suspend vendors" }
            );
        }

        var vendor = await _context.Vendors
            .Include(v => v.VerificationDocuments)
            .FirstOrDefaultAsync(v => v.Id == request.VendorId, cancellationToken);

        if (vendor == null)
        {
            return Result<VendorDto>.Failure(
                "Vendor not found",
                new List<string> { "Vendor does not exist" }
            );
        }

        vendor.VerificationStatus = VerificationStatus.Suspended;
        vendor.IsActive = false;
        vendor.AcceptOrders = false;
        vendor.VerificationNotes = $"Suspended: {request.Reason}";
        vendor.UpdatedAt = DateTime.UtcNow;
        vendor.UpdatedBy = adminUser.Email;

        await _context.SaveChangesAsync(cancellationToken);

        var vendorDto = MapToDto(vendor);

        return Result<VendorDto>.Success(vendorDto, "Vendor suspended successfully");
    }

    private VendorDto MapToDto(Domain.Entities.Vendor vendor)
    {
        return new VendorDto
        {
            Id = vendor.Id,
            UserId = vendor.UserId,
            BusinessName = vendor.BusinessName,
            BusinessDescription = vendor.BusinessDescription,
            BusinessEmail = vendor.BusinessEmail,
            BusinessPhone = vendor.BusinessPhone,
            Website = vendor.Website,
            BusinessAddress = !string.IsNullOrEmpty(vendor.Street) ? new VendorAddressDto
            {
                Street = vendor.Street,
                City = vendor.City ?? string.Empty,
                State = vendor.State ?? string.Empty,
                ZipCode = vendor.ZipCode ?? string.Empty,
                Country = vendor.Country ?? string.Empty
            } : null,
            TaxId = vendor.TaxId,
            RegistrationNumber = vendor.RegistrationNumber,
            Logo = vendor.Logo,
            CoverImage = vendor.CoverImage,
            VerificationStatus = vendor.VerificationStatus,
            VerifiedAt = vendor.VerifiedAt,
            VerificationNotes = vendor.VerificationNotes,
            Rating = vendor.Rating,
            TotalReviews = vendor.TotalReviews,
            TotalSales = vendor.TotalSales,
            TotalRevenue = vendor.TotalRevenue,
            TotalProducts = vendor.TotalProducts,
            IsActive = vendor.IsActive,
            AcceptOrders = vendor.AcceptOrders,
            AutoApproveReviews = vendor.AutoApproveReviews,
            VerificationDocuments = vendor.VerificationDocuments.Select(d => new VendorVerificationDto
            {
                Id = d.Id,
                DocumentType = d.DocumentType,
                FileName = d.FileName,
                FilePath = d.FilePath,
                Status = d.Status,
                RejectionReason = d.RejectionReason,
                ReviewedAt = d.ReviewedAt,
                CreatedAt = d.CreatedAt
            }).ToList(),
            CreatedAt = vendor.CreatedAt,
            UpdatedAt = vendor.UpdatedAt
        };
    }
}
