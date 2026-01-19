using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Commands.TaxInfo.CreateTaxInfo;

public class CreateTaxInfoCommand : IRequest<Result<TaxInfoResponse>>
{
    public string TaxId { get; set; } = string.Empty;
    public TaxIdType TaxIdType { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? State { get; set; }
    public decimal? TaxRate { get; set; }
    public bool CollectsTax { get; set; } = false;
}

public class TaxInfoResponse
{
    public Guid Id { get; set; }
    public string MaskedTaxId { get; set; } = string.Empty;
    public TaxIdType TaxIdType { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? State { get; set; }
    public bool IsVerified { get; set; }
    public bool CollectsTax { get; set; }
}
