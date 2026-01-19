using Marketplace.Application.Features.Cart.Commands.AddToCart;
using Marketplace.Application.Features.Cart.Commands.ClearCart;
using Marketplace.Application.Features.Cart.Commands.RemoveFromCart;
using Marketplace.Application.Features.Cart.Commands.UpdateCartItemQuantity;
using Marketplace.Application.Features.Cart.Queries.GetCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userIdString = User.Identity?.IsAuthenticated == true ? User.FindFirst("sub")?.Value : null;
        Guid? userId = null;
        if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var parsedUserId))
        {
            userId = parsedUserId;
        }
        var sessionId = Request.Cookies["session_id"] ?? Guid.NewGuid().ToString();

        // Set session cookie if not exists
        if (string.IsNullOrEmpty(Request.Cookies["session_id"]))
        {
            Response.Cookies.Append("session_id", sessionId, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Set to true in production with HTTPS
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });
        }

        var query = new GetCartQuery
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

    [HttpPost("items")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        var userIdString = User.Identity?.IsAuthenticated == true ? User.FindFirst("sub")?.Value : null;
        Guid? userId = null;
        if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var parsedUserId))
        {
            userId = parsedUserId;
        }
        var sessionId = Request.Cookies["session_id"];

        var command = new AddToCartCommand
        {
            ProductId = request.ProductId,
            ProductVariationId = request.ProductVariationId,
            Quantity = request.Quantity,
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

    [HttpPut("items/{cartItemId}")]
    public async Task<IActionResult> UpdateCartItemQuantity(Guid cartItemId, [FromBody] UpdateCartItemQuantityRequest request)
    {
        var userIdString = User.Identity?.IsAuthenticated == true ? User.FindFirst("sub")?.Value : null;
        Guid? userId = null;
        if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var parsedUserId))
        {
            userId = parsedUserId;
        }
        var sessionId = Request.Cookies["session_id"];

        var command = new UpdateCartItemQuantityCommand
        {
            CartItemId = cartItemId,
            Quantity = request.Quantity,
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

    [HttpDelete("items/{cartItemId}")]
    public async Task<IActionResult> RemoveFromCart(Guid cartItemId)
    {
        var userIdString = User.Identity?.IsAuthenticated == true ? User.FindFirst("sub")?.Value : null;
        Guid? userId = null;
        if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var parsedUserId))
        {
            userId = parsedUserId;
        }
        var sessionId = Request.Cookies["session_id"];

        var command = new RemoveFromCartCommand
        {
            CartItemId = cartItemId,
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

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var userIdString = User.Identity?.IsAuthenticated == true ? User.FindFirst("sub")?.Value : null;
        Guid? userId = null;
        if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var parsedUserId))
        {
            userId = parsedUserId;
        }
        var sessionId = Request.Cookies["session_id"];

        var command = new ClearCartCommand
        {
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

public class AddToCartRequest
{
    public Guid ProductId { get; set; }
    public Guid? ProductVariationId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class UpdateCartItemQuantityRequest
{
    public int Quantity { get; set; }
}