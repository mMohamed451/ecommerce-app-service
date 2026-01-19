using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class NotificationPreference : BaseEntity, IAuditable
{
    public Guid UserId { get; set; }
    public bool EmailNotifications { get; set; } = true;
    public bool SmsNotifications { get; set; } = false;
    public bool PushNotifications { get; set; } = true;
    public bool OrderUpdates { get; set; } = true;
    public bool PromotionalEmails { get; set; } = true;
    public bool SecurityAlerts { get; set; } = true;
    public bool MarketingEmails { get; set; } = false;
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation property
    public virtual ApplicationUser? User { get; set; }
}
