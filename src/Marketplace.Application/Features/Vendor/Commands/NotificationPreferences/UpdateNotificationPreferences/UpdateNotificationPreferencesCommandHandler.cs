using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Vendor.Commands.NotificationPreferences.UpdateNotificationPreferences;

public class UpdateNotificationPreferencesCommandHandler : IRequestHandler<UpdateNotificationPreferencesCommand, Result<NotificationPreferencesResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateNotificationPreferencesCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<NotificationPreferencesResponse>> Handle(UpdateNotificationPreferencesCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<NotificationPreferencesResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<NotificationPreferencesResponse>.Failure(
                "Vendor not found",
                new List<string> { "Vendor account does not exist" }
            );
        }

        var preferences = await _context.VendorNotificationPreferences
            .FirstOrDefaultAsync(p => p.VendorId == vendor.Id, cancellationToken);

        if (preferences == null)
        {
            preferences = new VendorNotificationPreference
            {
                VendorId = vendor.Id,
                CreatedBy = userId.Value.ToString()
            };
            _context.VendorNotificationPreferences.Add(preferences);
        }

        // Update only provided fields
        if (request.EmailOrderNotifications.HasValue)
            preferences.EmailOrderNotifications = request.EmailOrderNotifications.Value;
        if (request.EmailPaymentNotifications.HasValue)
            preferences.EmailPaymentNotifications = request.EmailPaymentNotifications.Value;
        if (request.EmailReviewNotifications.HasValue)
            preferences.EmailReviewNotifications = request.EmailReviewNotifications.Value;
        if (request.EmailProductNotifications.HasValue)
            preferences.EmailProductNotifications = request.EmailProductNotifications.Value;
        if (request.EmailMarketingEmails.HasValue)
            preferences.EmailMarketingEmails = request.EmailMarketingEmails.Value;
        if (request.SmsOrderNotifications.HasValue)
            preferences.SmsOrderNotifications = request.SmsOrderNotifications.Value;
        if (request.SmsPaymentNotifications.HasValue)
            preferences.SmsPaymentNotifications = request.SmsPaymentNotifications.Value;
        if (request.SmsUrgentAlerts.HasValue)
            preferences.SmsUrgentAlerts = request.SmsUrgentAlerts.Value;
        if (request.PushOrderNotifications.HasValue)
            preferences.PushOrderNotifications = request.PushOrderNotifications.Value;
        if (request.PushPaymentNotifications.HasValue)
            preferences.PushPaymentNotifications = request.PushPaymentNotifications.Value;
        if (request.PushReviewNotifications.HasValue)
            preferences.PushReviewNotifications = request.PushReviewNotifications.Value;
        if (request.InAppOrderNotifications.HasValue)
            preferences.InAppOrderNotifications = request.InAppOrderNotifications.Value;
        if (request.InAppPaymentNotifications.HasValue)
            preferences.InAppPaymentNotifications = request.InAppPaymentNotifications.Value;
        if (request.InAppReviewNotifications.HasValue)
            preferences.InAppReviewNotifications = request.InAppReviewNotifications.Value;
        if (request.InAppSystemNotifications.HasValue)
            preferences.InAppSystemNotifications = request.InAppSystemNotifications.Value;

        preferences.UpdatedBy = userId.Value.ToString();
        await _context.SaveChangesAsync(cancellationToken);

        return Result<NotificationPreferencesResponse>.Success(new NotificationPreferencesResponse
        {
            Id = preferences.Id,
            EmailOrderNotifications = preferences.EmailOrderNotifications,
            EmailPaymentNotifications = preferences.EmailPaymentNotifications,
            EmailReviewNotifications = preferences.EmailReviewNotifications,
            EmailProductNotifications = preferences.EmailProductNotifications,
            EmailMarketingEmails = preferences.EmailMarketingEmails,
            SmsOrderNotifications = preferences.SmsOrderNotifications,
            SmsPaymentNotifications = preferences.SmsPaymentNotifications,
            SmsUrgentAlerts = preferences.SmsUrgentAlerts,
            PushOrderNotifications = preferences.PushOrderNotifications,
            PushPaymentNotifications = preferences.PushPaymentNotifications,
            PushReviewNotifications = preferences.PushReviewNotifications,
            InAppOrderNotifications = preferences.InAppOrderNotifications,
            InAppPaymentNotifications = preferences.InAppPaymentNotifications,
            InAppReviewNotifications = preferences.InAppReviewNotifications,
            InAppSystemNotifications = preferences.InAppSystemNotifications
        });
    }
}
