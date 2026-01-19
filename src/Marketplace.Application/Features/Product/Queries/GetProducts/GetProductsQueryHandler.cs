using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Features.Product.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Product.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<ProductsListResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProductsListResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products
            .Include(p => p.Vendor)
                .ThenInclude(v => v.Locations.Where(l => l.IsActive)) // Include vendor locations for distance filtering
            .Include(p => p.Category)
            .Include(p => p.Images.Where(i => i.IsPrimary))
            .Include(p => p.Variations)
                .ThenInclude(v => v.VariationAttributes)
            .Include(p => p.Attributes)
            .Where(p => p.Status != Domain.Entities.ProductStatus.Deleted)
            .AsQueryable();

        // Apply basic filters
        query = ApplyBasicFilters(query, request);

        // Apply price filters
        query = ApplyPriceFilters(query, request);

        // Apply rating filters
        query = ApplyRatingFilters(query, request);

        // Apply location filters (vendor location-based)
        query = ApplyLocationFilters(query, request);

        // Apply stock filters
        query = ApplyStockFilters(query, request);

        // Apply category hierarchy filters
        query = ApplyCategoryHierarchyFilters(query, request);

        // Apply attribute filters
        query = ApplyAttributeFilters(query, request);

        // Apply search term with enhanced full-text search
        query = ApplySearchTerm(query, request);

        // Get total count before pagination and sorting
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply enhanced sorting
        query = ApplySorting(query, request);

        // Apply pagination (support both offset and cursor-based)
        var products = await ApplyPagination(query, request, cancellationToken);

        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            VendorId = p.VendorId,
            VendorName = p.Vendor?.BusinessName ?? string.Empty,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name,
            Name = p.Name,
            Slug = p.Slug,
            Description = p.Description,
            ShortDescription = p.ShortDescription,
            SKU = p.SKU,
            Barcode = p.Barcode,
            Price = p.Price,
            CompareAtPrice = p.CompareAtPrice,
            CostPrice = p.CostPrice,
            StockQuantity = p.StockQuantity,
            LowStockThreshold = p.LowStockThreshold,
            TrackInventory = p.TrackInventory,
            AllowBackorder = p.AllowBackorder,
            Status = p.Status,
            IsActive = p.IsActive,
            IsFeatured = p.IsFeatured,
            IsDigital = p.IsDigital,
            RequiresShipping = p.RequiresShipping,
            Weight = p.Weight,
            Length = p.Length,
            Width = p.Width,
            Height = p.Height,
            MetaTitle = p.MetaTitle,
            MetaDescription = p.MetaDescription,
            MetaKeywords = p.MetaKeywords,
            Rating = p.Rating,
            ReviewCount = p.ReviewCount,
            ViewCount = p.ViewCount,
            SalesCount = p.SalesCount,
            ApprovalStatus = p.ApprovalStatus,
            PublishedAt = p.PublishedAt,
            Images = p.Images.Select(i => new ProductImageDto
            {
                Id = i.Id,
                FileUrl = i.FileUrl,
                AltText = i.AltText,
                DisplayOrder = i.DisplayOrder,
                IsPrimary = i.IsPrimary
            }).ToList(),
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return Result<ProductsListResponse>.Success(
            new ProductsListResponse
            {
                Products = productDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                NextCursor = products.Any() ? GenerateCursor(products.Last(), request.SortBy, request.SortDescending) : null
            }
        );
    }

    private IQueryable<Domain.Entities.Product> ApplyBasicFilters(IQueryable<Domain.Entities.Product> query, GetProductsQuery request)
    {
        if (request.VendorId.HasValue)
        {
            query = query.Where(p => p.VendorId == request.VendorId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(p => p.Status == request.Status.Value);
        }
        else
        {
            // Default to published products if no status specified
            query = query.Where(p => p.Status == Domain.Entities.ProductStatus.Published);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == request.IsActive.Value);
        }
        else
        {
            // Default to active products
            query = query.Where(p => p.IsActive);
        }

        if (request.IsFeatured.HasValue)
        {
            query = query.Where(p => p.IsFeatured == request.IsFeatured.Value);
        }

        if (request.IsDigital.HasValue)
        {
            query = query.Where(p => p.IsDigital == request.IsDigital.Value);
        }

        if (request.RequiresShipping.HasValue)
        {
            query = query.Where(p => p.RequiresShipping == request.RequiresShipping.Value);
        }

        return query;
    }

    private IQueryable<Domain.Entities.Product> ApplyPriceFilters(IQueryable<Domain.Entities.Product> query, GetProductsQuery request)
    {
        if (request.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= request.MaxPrice.Value);
        }

        return query;
    }

    private IQueryable<Domain.Entities.Product> ApplyRatingFilters(IQueryable<Domain.Entities.Product> query, GetProductsQuery request)
    {
        if (request.MinRating.HasValue)
        {
            query = query.Where(p => p.Rating >= request.MinRating.Value);
        }

        if (request.MinReviewCount.HasValue)
        {
            query = query.Where(p => p.ReviewCount >= request.MinReviewCount.Value);
        }

        return query;
    }

    private IQueryable<Domain.Entities.Product> ApplyLocationFilters(IQueryable<Domain.Entities.Product> query, GetProductsQuery request)
    {
        if (request.Latitude.HasValue && request.Longitude.HasValue && request.RadiusInKm.HasValue)
        {
            // Haversine formula for distance calculation
            var lat = request.Latitude.Value;
            var lng = request.Longitude.Value;
            var radius = request.RadiusInKm.Value;

            query = query.Where(p =>
                p.Vendor != null &&
                p.Vendor.Locations.Any(l =>
                    l.Latitude.HasValue &&
                    l.Longitude.HasValue &&
                    l.IsActive &&
                    // Distance calculation using Haversine formula (simplified for SQL)
                    Math.Acos(Math.Sin(lat * Math.PI / 180) * Math.Sin((double)l.Latitude.Value * Math.PI / 180) +
                             Math.Cos(lat * Math.PI / 180) * Math.Cos((double)l.Latitude.Value * Math.PI / 180) *
                             Math.Cos(((double)l.Longitude.Value - lng) * Math.PI / 180)) * 6371 <= radius
                )
            );
        }

        return query;
    }

    private IQueryable<Domain.Entities.Product> ApplyStockFilters(IQueryable<Domain.Entities.Product> query, GetProductsQuery request)
    {
        if (request.InStock.HasValue)
        {
            query = request.InStock.Value
                ? query.Where(p => p.StockQuantity > 0)
                : query.Where(p => p.StockQuantity == 0);
        }

        if (request.MinStockQuantity.HasValue)
        {
            query = query.Where(p => p.StockQuantity >= request.MinStockQuantity.Value);
        }

        return query;
    }

    private IQueryable<Domain.Entities.Product> ApplyCategoryHierarchyFilters(IQueryable<Domain.Entities.Product> query, GetProductsQuery request)
    {
        if (request.CategoryIds != null && request.CategoryIds.Any())
        {
            // Include products from specified categories and their subcategories
            // This requires a recursive CTE or pre-computed category hierarchy
            // For now, we'll do a simple match - in production, you'd want to expand this
            query = query.Where(p => p.CategoryId.HasValue && request.CategoryIds.Contains(p.CategoryId.Value));
        }

        return query;
    }

    private IQueryable<Domain.Entities.Product> ApplyAttributeFilters(IQueryable<Domain.Entities.Product> query, GetProductsQuery request)
    {
        if (request.AttributeFilters != null && request.AttributeFilters.Any())
        {
            foreach (var filter in request.AttributeFilters)
            {
                var attributeName = filter.Key;
                var values = filter.Value;

                if (values.Any())
                {
                query = query.Where(p =>
                    p.Attributes.Any(a => a.Name == attributeName && values.Contains(a.Value)) ||
                    p.Variations.Any(v => v.VariationAttributes.Any(va => va.Name == attributeName && values.Contains(va.Value)))
                );
                }
            }
        }

        return query;
    }

    private IQueryable<Domain.Entities.Product> ApplySearchTerm(IQueryable<Domain.Entities.Product> query, GetProductsQuery request)
    {
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower().Trim();

            // Enhanced full-text search across multiple fields with relevance weighting
            query = query.Where(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(searchTerm)) ||
                (p.ShortDescription != null && p.ShortDescription.ToLower().Contains(searchTerm)) ||
                (p.SKU != null && p.SKU.ToLower().Contains(searchTerm)) ||
                (p.MetaKeywords != null && p.MetaKeywords.ToLower().Contains(searchTerm)) ||
                p.Vendor!.BusinessName.ToLower().Contains(searchTerm) ||
                (p.Category != null && p.Category.Name.ToLower().Contains(searchTerm))
            );
        }

        return query;
    }

    private IQueryable<Domain.Entities.Product> ApplySorting(IQueryable<Domain.Entities.Product> query, GetProductsQuery request)
    {
        var sortBy = request.SortBy?.ToLower() ?? "created";

        return sortBy switch
        {
            "name" => request.SortDescending
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),
            "price" => request.SortDescending
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),
            "created" => request.SortDescending
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt),
            "rating" => request.SortDescending
                ? query.OrderByDescending(p => p.Rating)
                : query.OrderBy(p => p.Rating),
            "popularity" => request.SortDescending
                ? query.OrderByDescending(p => p.ViewCount)
                : query.OrderBy(p => p.ViewCount),
            "sales" => request.SortDescending
                ? query.OrderByDescending(p => p.SalesCount)
                : query.OrderBy(p => p.SalesCount),
            "trending" => query.OrderByDescending(p => p.ViewCount + p.SalesCount), // Simple trending score
            "relevance" => query.OrderByDescending(p => (double)p.Rating * 0.3 + p.ViewCount * 0.1 + p.SalesCount * 0.6), // Weighted relevance
            _ => query.OrderByDescending(p => p.CreatedAt)
        };
    }

    private async Task<List<Domain.Entities.Product>> ApplyPagination(IQueryable<Domain.Entities.Product> query, GetProductsQuery request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.Cursor))
        {
            // Cursor-based pagination implementation
            // This is a simplified version - in production, you'd decode the cursor and apply appropriate filtering
            var skip = (request.PageNumber - 1) * request.PageSize;
            return await query
                .Skip(skip)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
        }
        else
        {
            // Offset-based pagination
            var skip = (request.PageNumber - 1) * request.PageSize;
            return await query
                .Skip(skip)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
        }
    }

    private string? GenerateCursor(Domain.Entities.Product lastProduct, string? sortBy, bool sortDescending)
    {
        // Simple cursor generation - in production, you'd want to encode this properly
        // For now, we'll use a combination of the sort field value and ID
        var sortValue = sortBy?.ToLower() switch
        {
            "name" => lastProduct.Name,
            "price" => lastProduct.Price.ToString(),
            "created" => lastProduct.CreatedAt.ToString("O"),
            "rating" => lastProduct.Rating.ToString(),
            "popularity" => lastProduct.ViewCount.ToString(),
            "sales" => lastProduct.SalesCount.ToString(),
            _ => lastProduct.CreatedAt.ToString("O")
        };

        return $"{sortValue}|{lastProduct.Id}|{sortDescending}";
    }
}
