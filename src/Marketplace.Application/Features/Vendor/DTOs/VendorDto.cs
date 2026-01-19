using Marketplace.Domain.Entities;

namespace Marketplace.Application.Features.Vendor.DTOs;

public class VendorDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string? BusinessDescription { get; set; }
    public string BusinessEmail { get; set; } = string.Empty;
    public string BusinessPhone { get; set; } = string.Empty;
    public string? Website { get; set; }
    public VendorAddressDto? BusinessAddress { get; set; }
    public string? TaxId { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Logo { get; set; }
    public string? CoverImage { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerificationNotes { get; set; }
    public decimal Rating { get; set; }
    public int TotalReviews { get; set; }
    public int TotalSales { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalProducts { get; set; }
    public bool IsActive { get; set; }
    public bool AcceptOrders { get; set; }
    public bool AutoApproveReviews { get; set; }
    public List<VendorVerificationDto>? VerificationDocuments { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class VendorAddressDto
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public class VendorVerificationDto
{
    public Guid Id { get; set; }
    public DocumentType DocumentType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class VendorAnalyticsDto
{
    public int TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int TotalViews { get; set; }
    public decimal ConversionRate { get; set; }
    public List<SalesByPeriodDto> SalesByPeriod { get; set; } = new();
    public List<TopProductDto> TopProducts { get; set; } = new();
}

public class SalesByPeriodDto
{
    public string Period { get; set; } = string.Empty;
    public int Sales { get; set; }
    public int Orders { get; set; }
    public decimal Revenue { get; set; }
}

public class TopProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Sales { get; set; }
    public decimal Revenue { get; set; }
    public int Views { get; set; }
}

public class VendorReviewSummaryDto
{
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public RatingDistributionDto RatingDistribution { get; set; } = new();
}

public class RatingDistributionDto
{
    public int Five { get; set; }
    public int Four { get; set; }
    public int Three { get; set; }
    public int Two { get; set; }
    public int One { get; set; }
}
