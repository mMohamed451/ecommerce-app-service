using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Marketplace.Application.Features.Product.Commands.ExportProducts;

public class ExportProductsCommandHandler : IRequestHandler<ExportProductsCommand, Result<ExportProductsResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ExportProductsCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ExportProductsResponse>> Handle(ExportProductsCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<ExportProductsResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        // Get vendor
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<ExportProductsResponse>.Failure(
                "Vendor not found",
                new List<string> { "You must be a registered vendor to export products" }
            );
        }

        var query = _context.Products
            .Include(p => p.Category)
            .Where(p => p.VendorId == vendor.Id && p.Status != Domain.Entities.ProductStatus.Deleted)
            .AsQueryable();

        // Apply filters
        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(p => p.Status == request.Status.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == request.IsActive.Value);
        }

        var products = await query
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        // Generate CSV
        var csv = new StringBuilder();
        
        // Header
        csv.AppendLine("Name,SKU,Barcode,Description,Category,Price,CompareAtPrice,StockQuantity,Status,IsActive,IsDigital,RequiresShipping,Weight,Length,Width,Height");

        // Data rows
        foreach (var product in products)
        {
            csv.AppendLine($"{EscapeCsvField(product.Name)}," +
                          $"{EscapeCsvField(product.SKU ?? "")}," +
                          $"{EscapeCsvField(product.Barcode ?? "")}," +
                          $"{EscapeCsvField(product.Description ?? "")}," +
                          $"{EscapeCsvField(product.Category?.Name ?? "")}," +
                          $"{product.Price}," +
                          $"{product.CompareAtPrice?.ToString() ?? ""}," +
                          $"{product.StockQuantity}," +
                          $"{product.Status}," +
                          $"{product.IsActive}," +
                          $"{product.IsDigital}," +
                          $"{product.RequiresShipping}," +
                          $"{product.Weight?.ToString() ?? ""}," +
                          $"{product.Length?.ToString() ?? ""}," +
                          $"{product.Width?.ToString() ?? ""}," +
                          $"{product.Height?.ToString() ?? ""}");
        }

        var fileName = $"products_export_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        var fileContent = Encoding.UTF8.GetBytes(csv.ToString());

        return Result<ExportProductsResponse>.Success(
            new ExportProductsResponse
            {
                FileContent = fileContent,
                FileName = fileName,
                ContentType = "text/csv"
            },
            "Products exported successfully"
        );
    }

    private static string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
            return "";

        // If field contains comma, quote, or newline, wrap in quotes and escape quotes
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return field;
    }
}
