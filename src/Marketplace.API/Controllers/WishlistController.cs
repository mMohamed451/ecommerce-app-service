using Marketplace.Application.Features.Cart.Commands.AddToWishlist;
using Marketplace.Application.Features.Cart.Commands.RemoveFromWishlist;
using Marketplace.Application.Features.Cart.Queries.GetWishlist;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WishlistController : ControllerBase
{
    private readonly IMediator _mediator;

    public WishlistController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetWishlist()
    {
        var userIdString = User.Identity?.IsAuthenticated == true ? User.FindFirst("sub")?.Value : null;
        Guid? userId = null;
        if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var parsedUserId))
        {
            userId = parsedUserId;
        }
        var sessionId = Request.Cookies["session_id"];

        var query = new GetWishlistQuery
        {
            UserId = userId,
            SessionId = sessionId
        };

        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistRequest request)
    {
        var userIdString = User.Identity?.IsAuthenticated == true ? User.FindFirst("sub")?.Value : null;
        Guid? userId = null;
        if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var parsedUserId))
        {
            userId = parsedUserId;
        }
        var sessionId = Request.Cookies["session_id"];

        var command = new AddToWishlistCommand
        {
            ProductId = request.ProductId,
            UserId = userId,
            SessionId = sessionId
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveFromWishlist(Guid productId)
    {
        var userIdString = User.Identity?.IsAuthenticated == true ? User.FindFirst("sub")?.Value : null;
        Guid? userId = null;
        if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var parsedUserId))
        {
            userId = parsedUserId;
        }
        var sessionId = Request.Cookies["session_id"];

        var command = new RemoveFromWishlistCommand
        {
            ProductId = productId,
            UserId = userId,
            SessionId = sessionId
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}

public class AddToWishlistRequest
{
    public Guid ProductId { get; set; }
}