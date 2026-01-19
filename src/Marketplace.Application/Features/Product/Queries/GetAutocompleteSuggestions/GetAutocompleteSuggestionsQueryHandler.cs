using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Product.Queries.GetAutocompleteSuggestions;

public class GetAutocompleteSuggestionsQueryHandler : IRequestHandler<GetAutocompleteSuggestionsQuery, Result<AutocompleteSuggestionsResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetAutocompleteSuggestionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AutocompleteSuggestionsResponse>> Handle(GetAutocompleteSuggestionsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query) || request.Query.Length < 2)
        {
            return Result<AutocompleteSuggestionsResponse>.Success(new AutocompleteSuggestionsResponse());
        }

        var suggestions = new List<AutocompleteSuggestion>();
        var query = request.Query.ToLower().Trim();

        // Get product suggestions
        if (request.IncludeProducts)
        {
            var productSuggestions = await _context.Products
                .Where(p => p.Status == Domain.Entities.ProductStatus.Published &&
                           p.IsActive &&
                           (p.Name.ToLower().Contains(query) ||
                            p.SKU != null && p.SKU.ToLower().Contains(query)))
                .OrderByDescending(p => p.SalesCount) // Prioritize popular products
                .Take(request.MaxSuggestions / 3 + 1) // Reserve slots for different types
                .Select(p => new AutocompleteSuggestion
                {
                    Text = p.Name,
                    Type = "product",
                    Id = p.Id,
                    RelevanceScore = CalculateRelevanceScore(query, p.Name, p.SalesCount, p.ViewCount),
                    Category = p.Category != null ? p.Category.Name : null,
                    VendorName = p.Vendor != null ? p.Vendor.BusinessName : null
                })
                .ToListAsync(cancellationToken);

            suggestions.AddRange(productSuggestions);
        }

        // Get category suggestions
        if (request.IncludeCategories)
        {
            var categorySuggestions = await _context.Categories
                .Where(c => c.Name.ToLower().Contains(query) && c.IsActive)
                .OrderByDescending(c => c.ProductCount) // Prioritize categories with more products
                .Take(request.MaxSuggestions / 3 + 1)
                .Select(c => new AutocompleteSuggestion
                {
                    Text = c.Name,
                    Type = "category",
                    Id = c.Id,
                    RelevanceScore = CalculateRelevanceScore(query, c.Name, c.ProductCount, 0)
                })
                .ToListAsync(cancellationToken);

            suggestions.AddRange(categorySuggestions);
        }

        // Get vendor suggestions
        if (request.IncludeVendors)
        {
            var vendorSuggestions = await _context.Vendors
                .Where(v => v.IsActive &&
                           v.VerificationStatus == Domain.Entities.VerificationStatus.Approved &&
                           v.BusinessName.ToLower().Contains(query))
                .OrderByDescending(v => v.TotalSales) // Prioritize popular vendors
                .Take(request.MaxSuggestions / 3 + 1)
                .Select(v => new AutocompleteSuggestion
                {
                    Text = v.BusinessName,
                    Type = "vendor",
                    Id = v.Id,
                    RelevanceScore = CalculateRelevanceScore(query, v.BusinessName, v.TotalSales, 0)
                })
                .ToListAsync(cancellationToken);

            suggestions.AddRange(vendorSuggestions);
        }

        // Sort by relevance score and limit total results
        var topSuggestions = suggestions
            .OrderByDescending(s => s.RelevanceScore)
            .Take(request.MaxSuggestions)
            .ToList();

        return Result<AutocompleteSuggestionsResponse>.Success(
            new AutocompleteSuggestionsResponse
            {
                Suggestions = topSuggestions
            }
        );
    }

    private int CalculateRelevanceScore(string query, string text, int popularityScore, int viewScore)
    {
        var textLower = text.ToLower();
        var score = 0;

        // Exact match gets highest score
        if (textLower.StartsWith(query))
        {
            score += 100;
        }
        // Starts with query (case insensitive)
        else if (textLower.Contains(query))
        {
            score += 50;
        }

        // Length bonus (shorter matches are often better)
        if (text.Length <= query.Length + 10)
        {
            score += 20;
        }

        // Popularity bonus
        score += Math.Min(popularityScore / 10, 20); // Cap at 20
        score += Math.Min(viewScore / 100, 10); // Cap at 10

        return score;
    }
}