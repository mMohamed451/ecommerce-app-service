using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class UserProfile : BaseEntity, IAuditable
{
    public Guid UserId { get; set; }
    public string Phone { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; } // Male, Female, Other
    public string? Bio { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation property
    public virtual ApplicationUser? User { get; set; }
}
