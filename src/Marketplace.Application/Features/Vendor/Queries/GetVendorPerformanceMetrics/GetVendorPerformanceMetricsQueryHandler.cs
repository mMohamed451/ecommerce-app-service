using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Vendor.Queries.GetVendorPerformanceMetrics;

public class GetVendorPerformanceMetricsQueryHandler : IRequestHandler<GetVendorPerformanceMetricsQuery, Result<PerformanceMetricsResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetVendorPerformanceMetricsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PerformanceMetricsResponse>> Handle(GetVendorPerformanceMetricsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<PerformanceMetricsResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<PerformanceMetricsResponse>.Failure(
                "Vendor not found",
                new List<string> { "Vendor account does not exist" }
            );
        }

        // Calculate date range based on period
        var (startDate, endDate) = CalculateDateRange(request.Period, request.StartDate, request.EndDate);

        // Get or create performance metrics
        var metrics = await _context.VendorPerformanceMetrics
            .FirstOrDefaultAsync(m => 
                m.VendorId == vendor.Id && 
                m.Period == request.Period &&
                m.PeriodStart == startDate &&
                m.PeriodEnd == endDate,
                cancellationToken);

        // If metrics don't exist, calculate them
        if (metrics == null)
        {
            // In a real implementation, you would calculate these from actual orders, reviews, etc.
            // For now, return basic metrics from vendor entity
            metrics = new VendorPerformanceMetrics
            {
                VendorId = vendor.Id,
                Period = request.Period,
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TotalOrders = vendor.TotalSales,
                AverageRating = vendor.Rating,
                TotalReviews = vendor.TotalReviews,
                TotalRevenue = vendor.TotalRevenue,
                TotalProducts = vendor.TotalProducts
            };
        }

        var response = new PerformanceMetricsResponse
        {
            VendorId = vendor.Id,
            Period = metrics.Period,
            PeriodStart = metrics.PeriodStart,
            PeriodEnd = metrics.PeriodEnd,
            TotalOrders = metrics.TotalOrders,
            FulfilledOrders = metrics.FulfilledOrders,
            CancelledOrders = metrics.CancelledOrders,
            ReturnedOrders = metrics.ReturnedOrders,
            OrderFulfillmentRate = metrics.OrderFulfillmentRate,
            AverageResponseTime = metrics.AverageResponseTime,
            TotalMessages = metrics.TotalMessages,
            RespondedMessages = metrics.RespondedMessages,
            ResponseRate = metrics.ResponseRate,
            AverageShippingTime = metrics.AverageShippingTime,
            OnTimeDeliveries = metrics.OnTimeDeliveries,
            LateDeliveries = metrics.LateDeliveries,
            OnTimeDeliveryRate = metrics.OnTimeDeliveryRate,
            AverageRating = metrics.AverageRating,
            TotalReviews = metrics.TotalReviews,
            PositiveReviews = metrics.PositiveReviews,
            NegativeReviews = metrics.NegativeReviews,
            ActiveProducts = metrics.ActiveProducts,
            TotalProducts = metrics.TotalProducts,
            LowStockProducts = metrics.LowStockProducts,
            TotalRevenue = metrics.TotalRevenue,
            PendingPayouts = metrics.PendingPayouts,
            TotalPayouts = metrics.TotalPayouts
        };

        return Result<PerformanceMetricsResponse>.Success(response);
    }

    private static (DateTime startDate, DateTime endDate) CalculateDateRange(
        MetricPeriod period,
        DateTime? customStartDate,
        DateTime? customEndDate)
    {
        if (customStartDate.HasValue && customEndDate.HasValue)
        {
            return (customStartDate.Value, customEndDate.Value);
        }

        var now = DateTime.UtcNow;
        return period switch
        {
            MetricPeriod.Daily => (now.Date, now.Date.AddDays(1).AddTicks(-1)),
            MetricPeriod.Weekly => (now.Date.AddDays(-(int)now.DayOfWeek), now.Date.AddDays(7).AddTicks(-1)),
            MetricPeriod.Monthly => (new DateTime(now.Year, now.Month, 1), new DateTime(now.Year, now.Month, 1).AddMonths(1).AddTicks(-1)),
            MetricPeriod.Yearly => (new DateTime(now.Year, 1, 1), new DateTime(now.Year, 12, 31, 23, 59, 59)),
            MetricPeriod.AllTime => (DateTime.MinValue, DateTime.MaxValue),
            _ => (now.Date, now.Date.AddDays(1).AddTicks(-1))
        };
    }
}
