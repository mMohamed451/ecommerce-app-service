using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Profile.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Profile.Queries.GetNotificationPreferences;

public class GetNotificationPreferencesQuery : IRequest<Result<NotificationPreferenceDto>>
{
}
