using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Profile.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<Result>
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
