using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Vendor.Commands.BusinessHours.UpdateBusinessHours;

public class UpdateBusinessHoursCommand : IRequest<Result<BusinessHoursResponse>>
{
    public List<DayHours> Days { get; set; } = new();
}

public class DayHours
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly? OpenTime { get; set; }
    public TimeOnly? CloseTime { get; set; }
    public bool IsClosed { get; set; }
    public bool Is24Hours { get; set; }
    public string? Notes { get; set; }
}

public class BusinessHoursResponse
{
    public List<DayHours> Days { get; set; } = new();
}
