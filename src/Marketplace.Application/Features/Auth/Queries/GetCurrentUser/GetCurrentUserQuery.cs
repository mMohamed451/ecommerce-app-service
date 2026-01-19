using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Auth.Commands.Register;
using MediatR;

namespace Marketplace.Application.Features.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQuery : IRequest<Result<UserDto>>
{
}
