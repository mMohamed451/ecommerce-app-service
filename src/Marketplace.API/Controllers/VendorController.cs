using Marketplace.Application.Features.Vendor.Commands.ApproveVendor;
using Marketplace.Application.Features.Vendor.Commands.DeactivateVendor;
using Marketplace.Application.Features.Vendor.Commands.RegisterVendor;
using Marketplace.Application.Features.Vendor.Commands.SuspendVendor;
using Marketplace.Application.Features.Vendor.Commands.UpdateVendorProfile;
using Marketplace.Application.Features.Vendor.Commands.UploadLogo;
using Marketplace.Application.Features.Vendor.Commands.UploadCoverImage;
using Marketplace.Application.Features.Vendor.Queries.GetVendorAnalytics;
using Marketplace.Application.Features.Vendor.Queries.GetVendorProfile;
using Marketplace.Application.Features.Vendor.Queries.GetVendorReviewSummary;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VendorController : ControllerBase
{
    private readonly IMediator _mediator;

    public VendorController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterVendorRequest request)
    {
        var command = new RegisterVendorCommand
        {
            BusinessName = request.BusinessName,
            BusinessDescription = request.BusinessDescription,
            BusinessEmail = request.BusinessEmail,
            BusinessPhone = request.BusinessPhone,
            Website = request.Website,
            Street = request.Street,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
            Country = request.Country,
            TaxId = request.TaxId,
            RegistrationNumber = request.RegistrationNumber,
            Documents = new List<DocumentUpload>()
        };

        // Process uploaded documents
        if (request.Documents != null && request.Documents.Count > 0)
        {
            foreach (var doc in request.Documents)
            {
                if (doc != null && doc.Length > 0)
                {
                    command.Documents.Add(new DocumentUpload
                    {
                        FileStream = doc.OpenReadStream(),
                        FileName = doc.FileName,
                        ContentType = doc.ContentType,
                        FileSize = doc.Length,
                        DocumentType = (DocumentType)ParseDocumentType(doc.FileName)
                    });
                }
            }
        }

        var result = await _mediator.Send(command);
        
        // Dispose streams
        foreach (var doc in command.Documents)
        {
            await doc.FileStream.DisposeAsync();
        }

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _mediator.Send(new GetVendorProfileQuery());
        return Ok(result);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateVendorProfileCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("logo")]
    public async Task<IActionResult> UploadLogo([FromForm] IFormFile logo)
    {
        if (logo == null || logo.Length == 0)
        {
            return BadRequest(new { message = "Logo file is required" });
        }

        var command = new UploadLogoCommand
        {
            FileStream = logo.OpenReadStream(),
            FileName = logo.FileName,
            ContentType = logo.ContentType,
            FileSize = logo.Length
        };

        var result = await _mediator.Send(command);
        
        await command.FileStream.DisposeAsync();
        
        return Ok(result);
    }

    [HttpPost("cover-image")]
    public async Task<IActionResult> UploadCoverImage([FromForm] IFormFile coverImage)
    {
        if (coverImage == null || coverImage.Length == 0)
        {
            return BadRequest(new { message = "Cover image file is required" });
        }

        var command = new UploadCoverImageCommand
        {
            FileStream = coverImage.OpenReadStream(),
            FileName = coverImage.FileName,
            ContentType = coverImage.ContentType,
            FileSize = coverImage.Length
        };

        var result = await _mediator.Send(command);
        
        await command.FileStream.DisposeAsync();
        
        return Ok(result);
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var query = new GetVendorAnalyticsQuery
        {
            StartDate = startDate,
            EndDate = endDate
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("ratings/summary")]
    public async Task<IActionResult> GetReviewSummary()
    {
        var result = await _mediator.Send(new GetVendorReviewSummaryQuery());
        return Ok(result);
    }

    [HttpPost("deactivate")]
    public async Task<IActionResult> Deactivate([FromBody] DeactivateVendorCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // Admin-only endpoints
    [HttpPost("approve")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Approve([FromBody] ApproveVendorCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("suspend")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Suspend([FromBody] SuspendVendorCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    private int ParseDocumentType(string fileName)
    {
        var lowerFileName = fileName.ToLowerInvariant();
        if (lowerFileName.Contains("license") || lowerFileName.Contains("business"))
            return (int)DocumentType.BusinessLicense;
        if (lowerFileName.Contains("tax"))
            return (int)DocumentType.TaxCertificate;
        if (lowerFileName.Contains("id") || lowerFileName.Contains("identity"))
            return (int)DocumentType.IdentityProof;
        if (lowerFileName.Contains("bank") || lowerFileName.Contains("statement"))
            return (int)DocumentType.BankStatement;
        return (int)DocumentType.Other;
    }
}

public class RegisterVendorRequest
{
    public string BusinessName { get; set; } = string.Empty;
    public string? BusinessDescription { get; set; }
    public string BusinessEmail { get; set; } = string.Empty;
    public string BusinessPhone { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public string? RegistrationNumber { get; set; }
    public List<IFormFile>? Documents { get; set; }
}
