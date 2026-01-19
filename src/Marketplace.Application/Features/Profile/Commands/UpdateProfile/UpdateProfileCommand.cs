using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Profile.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Profile.Commands.UpdateProfile;

public class UpdateProfileCommand : IRequest<Result<UserProfileDto>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Bio { get; set; }
}
