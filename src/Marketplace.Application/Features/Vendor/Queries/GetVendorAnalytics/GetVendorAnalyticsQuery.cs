using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Vendor.DTOs;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Queries.GetVendorAnalytics;

public class GetVendorAnalyticsQuery : IRequest<Result<VendorAnalyticsDto>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
