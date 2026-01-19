using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Product.Commands.ApproveProduct;

public class ApproveProductCommand : IRequest<Result<bool>>
{
    public Guid ProductId { get; set; }
    public bool Approve { get; set; } = true;
    public string? RejectionReason { get; set; }
}
