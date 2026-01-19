using Marketplace.Application.Features.Profile.Commands.Addresses.CreateAddress;
using Marketplace.Application.Features.Profile.Commands.Addresses.DeleteAddress;
using Marketplace.Application.Features.Profile.Commands.Addresses.SetDefaultAddress;
using Marketplace.Application.Features.Profile.Commands.Addresses.UpdateAddress;
using Marketplace.Application.Features.Profile.Commands.ChangePassword;
using Marketplace.Application.Features.Profile.Commands.UpdateNotificationPreferences;
using Marketplace.Application.Features.Profile.Commands.UpdateProfile;
using Marketplace.Application.Features.Profile.Commands.UploadAvatar;
using Marketplace.Application.Features.Profile.Queries.GetAddresses;
using Marketplace.Application.Features.Profile.Queries.GetNotificationPreferences;
using Marketplace.Application.Features.Profile.Queries.GetProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _mediator.Send(new GetProfileQuery());
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar([FromForm] IFormFile avatar)
    {
        if (avatar == null || avatar.Length == 0)
        {
            return BadRequest(new { message = "Avatar file is required" });
        }

        var command = new UploadAvatarCommand
        {
            FileStream = avatar.OpenReadStream(),
            FileName = avatar.FileName,
            ContentType = avatar.ContentType,
            FileSize = avatar.Length
        };

        var result = await _mediator.Send(command);
        
        // Dispose the stream after command execution
        await command.FileStream.DisposeAsync();
        
        return Ok(result);
    }

    [HttpGet("addresses")]
    public async Task<IActionResult> GetAddresses()
    {
        var result = await _mediator.Send(new GetAddressesQuery());
        return Ok(result);
    }

    [HttpPost("addresses")]
    public async Task<IActionResult> CreateAddress([FromBody] CreateAddressCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("addresses/{id}")]
    public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] UpdateAddressCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("addresses/{id}")]
    public async Task<IActionResult> DeleteAddress(Guid id)
    {
        var command = new DeleteAddressCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("addresses/{id}/set-default")]
    public async Task<IActionResult> SetDefaultAddress(Guid id)
    {
        var command = new SetDefaultAddressCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotificationPreferences()
    {
        var result = await _mediator.Send(new GetNotificationPreferencesQuery());
        return Ok(result);
    }

    [HttpPut("notifications")]
    public async Task<IActionResult> UpdateNotificationPreferences([FromBody] UpdateNotificationPreferencesCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
