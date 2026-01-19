using Marketplace.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Marketplace.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>, IAuditable
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual UserProfile? Profile { get; set; }
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    public virtual NotificationPreference? NotificationPreference { get; set; }
    public virtual Vendor? Vendor { get; set; }
}
