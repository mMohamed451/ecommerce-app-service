using Marketplace.Application.Common.Helpers;
using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Product.Commands.CreateProduct;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Marketplace.Application.Features.Product.Commands.ImportProducts;

public class ImportProductsCommandHandler : IRequestHandler<ImportProductsCommand, Result<ImportProductsResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public ImportProductsCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IMediator mediator)
    {
        _context = context;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<Result<ImportProductsResponse>> Handle(ImportProductsCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<ImportProductsResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        // Get vendor
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);

        if (vendor == null)
        {
            return Result<ImportProductsResponse>.Failure(
                "Vendor not found",
                new List<string> { "You must be a registered vendor to import products" }
            );
        }

        var response = new ImportProductsResponse();
        var errors = new List<string>();

        try
        {
            // Read CSV file
            using var reader = new StreamReader(request.FileStream, Encoding.UTF8);
            var lines = new List<string>();
            
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    lines.Add(line);
                }
            }

            if (lines.Count < 2)
            {
                return Result<ImportProductsResponse>.Failure(
                    "Invalid file",
                    new List<string> { "CSV file must contain at least a header row and one data row" }
                );
            }

            // Skip header row
            for (int i = 1; i < lines.Count; i++)
            {
                response.TotalProcessed++;
                
                try
                {
                    var fields = ParseCsvLine(lines[i]);
                    
                    if (fields.Length < 4) // Minimum: Name, Price, StockQuantity, Status
                    {
                        errors.Add($"Row {i + 1}: Insufficient columns");
                        response.ErrorCount++;
                        if (!request.SkipErrors) break;
                        continue;
                    }

                    // Parse fields (simplified - adjust based on your CSV format)
                    var name = fields[0];
                    var sku = fields.Length > 1 ? fields[1] : null;
                    var description = fields.Length > 2 ? fields[2] : null;
                    var priceStr = fields.Length > 3 ? fields[3] : "0";
                    
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        errors.Add($"Row {i + 1}: Product name is required");
                        response.ErrorCount++;
                        if (!request.SkipErrors) break;
                        continue;
                    }

                    if (!decimal.TryParse(priceStr, out var price) || price <= 0)
                    {
                        errors.Add($"Row {i + 1}: Invalid price");
                        response.ErrorCount++;
                        if (!request.SkipErrors) break;
                        continue;
                    }

                    var stockQuantity = 0;
                    if (fields.Length > 4 && int.TryParse(fields[4], out var stock))
                    {
                        stockQuantity = stock;
                    }

                    // Create product command
                    var createCommand = new CreateProductCommand
                    {
                        Name = name,
                        Description = description,
                        SKU = sku,
                        Price = price,
                        StockQuantity = stockQuantity,
                        TrackInventory = true
                    };

                    var result = await _mediator.Send(createCommand, cancellationToken);
                    
                    if (result.IsSuccess)
                    {
                        response.SuccessCount++;
                    }
                    else
                    {
                        errors.Add($"Row {i + 1}: {result.Message}");
                        response.ErrorCount++;
                        if (!request.SkipErrors) break;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Row {i + 1}: {ex.Message}");
                    response.ErrorCount++;
                    if (!request.SkipErrors) break;
                }
            }

            response.Errors = errors;
            
            return Result<ImportProductsResponse>.Success(
                response,
                $"Import completed. Processed: {response.TotalProcessed}, Success: {response.SuccessCount}, Errors: {response.ErrorCount}"
            );
        }
        catch (Exception ex)
        {
            return Result<ImportProductsResponse>.Failure(
                "Import failed",
                new List<string> { ex.Message }
            );
        }
    }

    private static string[] ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var currentField = new StringBuilder();
        var inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    // Escaped quote
                    currentField.Append('"');
                    i++;
                }
                else
                {
                    // Toggle quote state
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                // End of field
                fields.Add(currentField.ToString().Trim());
                currentField.Clear();
            }
            else
            {
                currentField.Append(c);
            }
        }

        // Add last field
        fields.Add(currentField.ToString().Trim());

        return fields.ToArray();
    }
}
