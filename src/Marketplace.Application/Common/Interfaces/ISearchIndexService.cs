namespace Marketplace.Application.Common.Interfaces;

public interface ISearchIndexService
{
    /// <summary>
    /// Index a product for search
    /// </summary>
    Task IndexProductAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a product from search index
    /// </summary>
    Task RemoveProductAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Re-index all products
    /// </summary>
    Task ReindexAllProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Search products
    /// </summary>
    Task<List<Guid>> SearchProductsAsync(string searchTerm, int limit = 20, CancellationToken cancellationToken = default);
}
