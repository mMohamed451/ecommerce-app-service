using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class Address : BaseEntity, IAuditable
{
    public Guid UserId { get; set; }
    public string Label { get; set; } = string.Empty; // Home, Work, etc.
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation property
    public virtual ApplicationUser? User { get; set; }
}
