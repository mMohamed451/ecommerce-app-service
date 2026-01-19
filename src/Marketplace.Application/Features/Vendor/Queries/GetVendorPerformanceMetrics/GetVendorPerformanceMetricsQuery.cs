using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Queries.GetVendorPerformanceMetrics;

public class GetVendorPerformanceMetricsQuery : IRequest<Result<PerformanceMetricsResponse>>
{
    public MetricPeriod Period { get; set; } = MetricPeriod.Monthly;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class PerformanceMetricsResponse
{
    public Guid VendorId { get; set; }
    public MetricPeriod Period { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    
    // Order metrics
    public int TotalOrders { get; set; }
    public int FulfilledOrders { get; set; }
    public int CancelledOrders { get; set; }
    public int ReturnedOrders { get; set; }
    public decimal OrderFulfillmentRate { get; set; }
    
    // Response metrics
    public TimeSpan? AverageResponseTime { get; set; }
    public int TotalMessages { get; set; }
    public int RespondedMessages { get; set; }
    public decimal ResponseRate { get; set; }
    
    // Shipping metrics
    public TimeSpan? AverageShippingTime { get; set; }
    public int OnTimeDeliveries { get; set; }
    public int LateDeliveries { get; set; }
    public decimal OnTimeDeliveryRate { get; set; }
    
    // Review metrics
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int PositiveReviews { get; set; }
    public int NegativeReviews { get; set; }
    
    // Product metrics
    public int ActiveProducts { get; set; }
    public int TotalProducts { get; set; }
    public int LowStockProducts { get; set; }
    
    // Financial metrics
    public decimal TotalRevenue { get; set; }
    public decimal PendingPayouts { get; set; }
    public decimal TotalPayouts { get; set; }
}
