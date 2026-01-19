using Marketplace.Domain.Common;

namespace Marketplace.Domain.Entities;

public class VendorPerformanceMetrics : BaseEntity, IAuditable
{
    public Guid VendorId { get; set; }
    
    // Order metrics
    public int TotalOrders { get; set; } = 0;
    public int FulfilledOrders { get; set; } = 0;
    public int CancelledOrders { get; set; } = 0;
    public int ReturnedOrders { get; set; } = 0;
    public decimal OrderFulfillmentRate { get; set; } = 0; // Percentage
    
    // Response metrics
    public TimeSpan? AverageResponseTime { get; set; }
    public int TotalMessages { get; set; } = 0;
    public int RespondedMessages { get; set; } = 0;
    public decimal ResponseRate { get; set; } = 0; // Percentage
    
    // Shipping metrics
    public TimeSpan? AverageShippingTime { get; set; }
    public int OnTimeDeliveries { get; set; } = 0;
    public int LateDeliveries { get; set; } = 0;
    public decimal OnTimeDeliveryRate { get; set; } = 0; // Percentage
    
    // Review metrics
    public decimal AverageRating { get; set; } = 0;
    public int TotalReviews { get; set; } = 0;
    public int PositiveReviews { get; set; } = 0;
    public int NegativeReviews { get; set; } = 0;
    
    // Product metrics
    public int ActiveProducts { get; set; } = 0;
    public int TotalProducts { get; set; } = 0;
    public int LowStockProducts { get; set; } = 0;
    
    // Financial metrics
    public decimal TotalRevenue { get; set; } = 0;
    public decimal PendingPayouts { get; set; } = 0;
    public decimal TotalPayouts { get; set; } = 0;
    
    // Period
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public MetricPeriod Period { get; set; } = MetricPeriod.Daily;
    
    // Audit
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation property
    public virtual Vendor? Vendor { get; set; }
}

public enum MetricPeriod
{
    Daily = 0,
    Weekly = 1,
    Monthly = 2,
    Yearly = 3,
    AllTime = 4
}
