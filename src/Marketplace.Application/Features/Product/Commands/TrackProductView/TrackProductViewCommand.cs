using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Product.Commands.TrackProductView;

public class TrackProductViewCommand : IRequest<Result<bool>>
{
    public Guid ProductId { get; set; }
    public string? UserId { get; set; } // Optional: track which user viewed the product
    public string? SessionId { get; set; } // For anonymous users, track by session
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Referrer { get; set; }
}