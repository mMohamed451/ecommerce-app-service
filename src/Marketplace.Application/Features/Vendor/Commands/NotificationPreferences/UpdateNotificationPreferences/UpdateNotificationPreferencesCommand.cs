using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Commands.NotificationPreferences.UpdateNotificationPreferences;

public class UpdateNotificationPreferencesCommand : IRequest<Result<NotificationPreferencesResponse>>
{
    // Email notifications
    public bool? EmailOrderNotifications { get; set; }
    public bool? EmailPaymentNotifications { get; set; }
    public bool? EmailReviewNotifications { get; set; }
    public bool? EmailProductNotifications { get; set; }
    public bool? EmailMarketingEmails { get; set; }
    
    // SMS notifications
    public bool? SmsOrderNotifications { get; set; }
    public bool? SmsPaymentNotifications { get; set; }
    public bool? SmsUrgentAlerts { get; set; }
    
    // Push notifications
    public bool? PushOrderNotifications { get; set; }
    public bool? PushPaymentNotifications { get; set; }
    public bool? PushReviewNotifications { get; set; }
    
    // In-app notifications
    public bool? InAppOrderNotifications { get; set; }
    public bool? InAppPaymentNotifications { get; set; }
    public bool? InAppReviewNotifications { get; set; }
    public bool? InAppSystemNotifications { get; set; }
}

public class NotificationPreferencesResponse
{
    public Guid Id { get; set; }
    public bool EmailOrderNotifications { get; set; }
    public bool EmailPaymentNotifications { get; set; }
    public bool EmailReviewNotifications { get; set; }
    public bool EmailProductNotifications { get; set; }
    public bool EmailMarketingEmails { get; set; }
    public bool SmsOrderNotifications { get; set; }
    public bool SmsPaymentNotifications { get; set; }
    public bool SmsUrgentAlerts { get; set; }
    public bool PushOrderNotifications { get; set; }
    public bool PushPaymentNotifications { get; set; }
    public bool PushReviewNotifications { get; set; }
    public bool InAppOrderNotifications { get; set; }
    public bool InAppPaymentNotifications { get; set; }
    public bool InAppReviewNotifications { get; set; }
    public bool InAppSystemNotifications { get; set; }
}
