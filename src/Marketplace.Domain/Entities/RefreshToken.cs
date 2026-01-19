using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? ReasonRevoked { get; set; }
    public bool IsActive => RevokedAt == null && ExpiresAt > DateTime.UtcNow;

    // Navigation property - will be configured in Infrastructure
    public virtual ApplicationUser? User { get; set; }
}
