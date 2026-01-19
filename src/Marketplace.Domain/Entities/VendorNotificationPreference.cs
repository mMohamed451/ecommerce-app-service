using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class VendorNotificationPreference : BaseEntity, IAuditable
{
    public Guid VendorId { get; set; }
    
    // Email notifications
    public bool EmailOrderNotifications { get; set; } = true;
    public bool EmailPaymentNotifications { get; set; } = true;
    public bool EmailReviewNotifications { get; set; } = true;
    public bool EmailProductNotifications { get; set; } = true;
    public bool EmailMarketingEmails { get; set; } = false;
    
    // SMS notifications
    public bool SmsOrderNotifications { get; set; } = false;
    public bool SmsPaymentNotifications { get; set; } = false;
    public bool SmsUrgentAlerts { get; set; } = true;
    
    // Push notifications
    public bool PushOrderNotifications { get; set; } = true;
    public bool PushPaymentNotifications { get; set; } = true;
    public bool PushReviewNotifications { get; set; } = true;
    
    // In-app notifications
    public bool InAppOrderNotifications { get; set; } = true;
    public bool InAppPaymentNotifications { get; set; } = true;
    public bool InAppReviewNotifications { get; set; } = true;
    public bool InAppSystemNotifications { get; set; } = true;
    
    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation property
    public virtual Vendor? Vendor { get; set; }
}
