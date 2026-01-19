using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Commands.RegisterVendor;

public class RegisterVendorCommand : IRequest<Result<VendorRegistrationResponse>>
{
    // Business Information
    public string BusinessName { get; set; } = string.Empty;
    public string? BusinessDescription { get; set; }
    public string BusinessEmail { get; set; } = string.Empty;
    public string BusinessPhone { get; set; } = string.Empty;
    public string? Website { get; set; }
    
    // Business Address
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    
    // Legal Information
    public string? TaxId { get; set; }
    public string? RegistrationNumber { get; set; }
    
    // Documents (will be handled separately in controller)
    public List<DocumentUpload> Documents { get; set; } = new();
}

public class DocumentUpload
{
    public Stream FileStream { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DocumentType DocumentType { get; set; }
}

public class VendorRegistrationResponse
{
    public Guid VendorId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public VerificationStatus VerificationStatus { get; set; }
}
