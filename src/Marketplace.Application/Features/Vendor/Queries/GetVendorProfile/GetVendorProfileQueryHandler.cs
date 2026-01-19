using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Vendor.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Vendor.Queries.GetVendorProfile;

public class GetVendorProfileQueryHandler : IRequestHandler<GetVendorProfileQuery, Result<VendorDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetVendorProfileQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<VendorDto>> Handle(GetVendorProfileQuery request, CancellationToken cancellationToken)
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
            .Include(v => v.VerificationDocuments)
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<VendorDto>.Failure(
                "Vendor not found",
                new List<string> { "Vendor profile does not exist" }
            );
        }

        var vendorDto = new VendorDto
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

        return Result<VendorDto>.Success(vendorDto, "Vendor profile retrieved successfully");
    }
}
