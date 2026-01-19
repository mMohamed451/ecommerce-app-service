using Marketplace.Application.Common.Models;
using MediatR;

namespace Marketplace.Application.Features.Product.Commands.UpdateInventory;

public class UpdateInventoryCommand : IRequest<Result<InventoryResponse>>
{
    public Guid ProductId { get; set; }
    public int? StockQuantity { get; set; }
    public int? LowStockThreshold { get; set; }
    public bool? TrackInventory { get; set; }
    public bool? AllowBackorder { get; set; }
}

public class InventoryResponse
{
    public Guid ProductId { get; set; }
    public int StockQuantity { get; set; }
    public int? LowStockThreshold { get; set; }
    public bool TrackInventory { get; set; }
    public bool AllowBackorder { get; set; }
    public bool IsLowStock { get; set; }
    public bool IsOutOfStock { get; set; }
}
