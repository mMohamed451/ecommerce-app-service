using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Profile.DTOs;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Profile.Queries.GetNotificationPreferences;

public class GetNotificationPreferencesQueryHandler : IRequestHandler<GetNotificationPreferencesQuery, Result<NotificationPreferenceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetNotificationPreferencesQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<NotificationPreferenceDto>> Handle(GetNotificationPreferencesQuery request, CancellationToken cancellationToken)
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

        // Create default preferences if none exist
        if (preferences == null)
        {
            preferences = new NotificationPreference
            {
                Id = Guid.NewGuid(),
                UserId = userId.Value,
                EmailNotifications = true,
                SmsNotifications = false,
                PushNotifications = true,
                OrderUpdates = true,
                PromotionalEmails = true,
                SecurityAlerts = true,
                MarketingEmails = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.NotificationPreferences.AddAsync(preferences, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

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

        return Result<NotificationPreferenceDto>.Success(preferencesDto);
    }
}
