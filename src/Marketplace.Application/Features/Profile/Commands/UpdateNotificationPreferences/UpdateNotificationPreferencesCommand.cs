using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Profile.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Profile.Commands.UpdateNotificationPreferences;

public class UpdateNotificationPreferencesCommand : IRequest<Result<NotificationPreferenceDto>>
{
    public bool EmailNotifications { get; set; }
    public bool SmsNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool OrderUpdates { get; set; }
    public bool PromotionalEmails { get; set; }
    public bool SecurityAlerts { get; set; }
    public bool MarketingEmails { get; set; }
}
