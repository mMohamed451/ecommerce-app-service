using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Commands.Commissions.CreateCommission;

public class CreateCommissionCommand : IRequest<Result<CommissionResponse>>
{
    public decimal CommissionPercentage { get; set; }
    public decimal? FixedFee { get; set; }
    public decimal? MinimumCommission { get; set; }
    public decimal? MaximumCommission { get; set; }
    public CommissionType CommissionType { get; set; } = CommissionType.Percentage;
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? Notes { get; set; }
}

public class CommissionResponse
{
    public Guid Id { get; set; }
    public decimal CommissionPercentage { get; set; }
    public decimal? FixedFee { get; set; }
    public CommissionType CommissionType { get; set; }
    public bool IsActive { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}
