using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Profile.DTOs;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Profile.Commands.UpdateNotificationPreferences;

public class UpdateNotificationPreferencesCommandHandler : IRequestHandler<UpdateNotificationPreferencesCommand, Result<NotificationPreferenceDto>>
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

    public async Task<Result<NotificationPreferenceDto>> Handle(UpdateNotificationPreferencesCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<NotificationPreferenceDto>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var preferences = await _context.NotificationPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId.Value, cancellationToken);

        if (preferences == null)
        {
            preferences = new NotificationPreference
            {
                Id = Guid.NewGuid(),
                UserId = userId.Value,
                CreatedAt = DateTime.UtcNow
            };
            await _context.NotificationPreferences.AddAsync(preferences, cancellationToken);
        }

        preferences.EmailNotifications = request.EmailNotifications;
        preferences.SmsNotifications = request.SmsNotifications;
        preferences.PushNotifications = request.PushNotifications;
        preferences.OrderUpdates = request.OrderUpdates;
        preferences.PromotionalEmails = request.PromotionalEmails;
        preferences.SecurityAlerts = request.SecurityAlerts;
        preferences.MarketingEmails = request.MarketingEmails;
        preferences.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var preferencesDto = new NotificationPreferenceDto
        {
            Id = preferences.Id,
            UserId = preferences.UserId,
            EmailNotifications = preferences.EmailNotifications,
            SmsNotifications = preferences.SmsNotifications,
            PushNotifications = preferences.PushNotifications,
            OrderUpdates = preferences.OrderUpdates,
            PromotionalEmails = preferences.PromotionalEmails,
            SecurityAlerts = preferences.SecurityAlerts,
            MarketingEmails = preferences.MarketingEmails,
            CreatedAt = preferences.CreatedAt,
            UpdatedAt = preferences.UpdatedAt
        };

        return Result<NotificationPreferenceDto>.Success(preferencesDto, "Notification preferences updated successfully");
    }
}
