using Marketplace.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Domain.Entities;

namespace Marketplace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public SeedController(
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [HttpPost("admin")]
    public async Task<IActionResult> SeedAdmin()
    {
        try
        {
            await SeedData.SeedRolesAsync(_roleManager);
            await SeedData.SeedAdminUserAsync(_userManager, _roleManager);
            
            var adminUser = await _userManager.FindByEmailAsync("admin@marketplace.com");
            if (adminUser != null)
            {
                var roles = await _userManager.GetRolesAsync(adminUser);
                return Ok(new
                {
                    success = true,
                    message = "Admin user seeded successfully",
                    user = new
                    {
                        email = adminUser.Email,
                        firstName = adminUser.FirstName,
                        lastName = adminUser.LastName,
                        roles = roles
                    }
                });
            }
            
            return BadRequest(new { success = false, message = "Failed to create admin user" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpGet("check-admin")]
    public async Task<IActionResult> CheckAdmin()
    {
        try
        {
            var adminUser = await _userManager.FindByEmailAsync("admin@marketplace.com");
            if (adminUser == null)
            {
                return NotFound(new
                {
                    exists = false,
                    message = "Admin user does not exist"
                });
            }

            var roles = await _userManager.GetRolesAsync(adminUser);
            return Ok(new
            {
                exists = true,
                user = new
                {
                    id = adminUser.Id,
                    email = adminUser.Email,
                    firstName = adminUser.FirstName,
                    lastName = adminUser.LastName,
                    isActive = adminUser.IsActive,
                    isEmailVerified = adminUser.IsEmailVerified,
                    roles = roles,
                    createdAt = adminUser.CreatedAt
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
