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
            .Include(p => p.Category)
            .Include(p => p.Images.Where(i => i.IsPrimary))
            .Where(p => p.Status != Domain.Entities.ProductStatus.Deleted)
            .AsQueryable();

        // Apply filters
        if (request.VendorId.HasValue)
        {
            query = query.Where(p => p.VendorId == request.VendorId.Value);
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(searchTerm)) ||
                (p.SKU != null && p.SKU.ToLower().Contains(searchTerm))
            );
        }

        if (request.Status.HasValue)
        {
            query = query.Where(p => p.Status == request.Status.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == request.IsActive.Value);
        }

        if (request.IsFeatured.HasValue)
        {
            query = query.Where(p => p.IsFeatured == request.IsFeatured.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = request.SortBy?.ToLower() switch
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
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        // Apply pagination
        var skip = (request.PageNumber - 1) * request.PageSize;
        var products = await query
            .Skip(skip)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

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
                TotalPages = totalPages
            }
        );
    }
}
