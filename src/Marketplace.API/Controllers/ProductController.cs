using Marketplace.Application.Features.Product.Commands.CreateProduct;
using Marketplace.Application.Features.Product.Commands.UpdateProduct;
using Marketplace.Application.Features.Product.Commands.DeleteProduct;
using Marketplace.Application.Features.Product.Commands.UploadProductImage;
using Marketplace.Application.Features.Product.Commands.UpdateInventory;
using Marketplace.Application.Features.Product.Commands.ExportProducts;
using Marketplace.Application.Features.Product.Commands.ImportProducts;
using Marketplace.Application.Features.Product.Queries.GetProduct;
using Marketplace.Application.Features.Product.Queries.GetProductBySlug;
using Marketplace.Application.Features.Product.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] GetProductsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var query = new GetProductQuery { ProductId = id };
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetProductBySlug(string slug)
    {
        var query = new GetProductBySlugQuery { Slug = slug };
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductCommand command)
    {
        command.ProductId = id;
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var command = new DeleteProductCommand { ProductId = id };
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("{id}/images")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> UploadProductImage(
        Guid id,
        [FromForm] IFormFile fileStream,
        [FromForm] int displayOrder = 0,
        [FromForm] bool isPrimary = false,
        [FromForm] string? altText = null)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            return BadRequest(new { message = "Image file is required" });
        }

        var command = new UploadProductImageCommand
        {
            ProductId = id,
            FileStream = fileStream.OpenReadStream(),
            FileName = fileStream.FileName ?? "image",
            ContentType = fileStream.ContentType ?? "image/jpeg",
            FileSize = fileStream.Length,
            DisplayOrder = displayOrder,
            IsPrimary = isPrimary,
            AltText = altText
        };

        var result = await _mediator.Send(command);
        
        // Dispose the stream after command execution
        await command.FileStream.DisposeAsync();
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPut("{id}/inventory")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> UpdateInventory(Guid id, [FromBody] UpdateInventoryCommand command)
    {
        command.ProductId = id;
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("export")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> ExportProducts([FromQuery] ExportProductsCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return File(result.Data.FileContent, result.Data.ContentType, result.Data.FileName);
    }

    [HttpPost("import")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> ImportProducts([FromForm] IFormFile fileStream, [FromForm] bool skipErrors = false)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            return BadRequest(new { message = "CSV file is required" });
        }

        var command = new ImportProductsCommand
        {
            FileStream = fileStream.OpenReadStream(),
            FileName = fileStream.FileName ?? "products.csv",
            SkipErrors = skipErrors
        };

        var result = await _mediator.Send(command);
        
        // Dispose the stream after command execution
        await command.FileStream.DisposeAsync();
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
