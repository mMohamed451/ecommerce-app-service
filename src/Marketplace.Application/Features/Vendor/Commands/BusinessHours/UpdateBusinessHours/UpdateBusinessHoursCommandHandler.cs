using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Vendor.Commands.BusinessHours.UpdateBusinessHours;

public class UpdateBusinessHoursCommandHandler : IRequestHandler<UpdateBusinessHoursCommand, Result<BusinessHoursResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateBusinessHoursCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BusinessHoursResponse>> Handle(UpdateBusinessHoursCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<BusinessHoursResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<BusinessHoursResponse>.Failure(
                "Vendor not found",
                new List<string> { "Vendor account does not exist" }
            );
        }

        // Remove existing business hours
        var existingHours = await _context.VendorBusinessHours
            .Where(bh => bh.VendorId == vendor.Id)
            .ToListAsync(cancellationToken);

        _context.VendorBusinessHours.RemoveRange(existingHours);

        // Add new business hours
        var businessHours = request.Days.Select(day => new VendorBusinessHours
        {
            VendorId = vendor.Id,
            DayOfWeek = day.DayOfWeek,
            OpenTime = day.OpenTime,
            CloseTime = day.CloseTime,
            IsClosed = day.IsClosed,
            Is24Hours = day.Is24Hours,
            Notes = day.Notes,
            CreatedBy = userId.Value.ToString()
        }).ToList();

        _context.VendorBusinessHours.AddRange(businessHours);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new BusinessHoursResponse
        {
            Days = businessHours.Select(bh => new DayHours
            {
                DayOfWeek = bh.DayOfWeek,
                OpenTime = bh.OpenTime,
                CloseTime = bh.CloseTime,
                IsClosed = bh.IsClosed,
                Is24Hours = bh.Is24Hours,
                Notes = bh.Notes
            }).ToList()
        };

        return Result<BusinessHoursResponse>.Success(response);
    }
}
