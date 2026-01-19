using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest<Result>
{
    public string Email { get; set; } = string.Empty;
}
