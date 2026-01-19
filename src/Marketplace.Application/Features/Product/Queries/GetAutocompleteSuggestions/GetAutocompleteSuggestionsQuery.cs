using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Product.Queries.GetAutocompleteSuggestions;

public class GetAutocompleteSuggestionsQuery : IRequest<Result<AutocompleteSuggestionsResponse>>
{
    public string Query { get; set; } = string.Empty;
    public int MaxSuggestions { get; set; } = 10;
    public bool IncludeProducts { get; set; } = true;
    public bool IncludeCategories { get; set; } = true;
    public bool IncludeVendors { get; set; } = true;
}

public class AutocompleteSuggestionsResponse
{
    public List<AutocompleteSuggestion> Suggestions { get; set; } = new();
}

public class AutocompleteSuggestion
{
    public string Text { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "product", "category", "vendor"
    public Guid? Id { get; set; }
    public int RelevanceScore { get; set; }
    public string? Category { get; set; } // For products, show their category
    public string? VendorName { get; set; } // For products, show their vendor
}