using Marketplace.Application.Common.Interfaces;
using Marketplace.Application.Common.Models;
using Marketplace.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Application.Features.Vendor.Commands.RegisterVendor;

public class RegisterVendorCommandHandler : IRequestHandler<RegisterVendorCommand, Result<VendorRegistrationResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IFileStorageService _fileStorageService;

    public RegisterVendorCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager,
        IFileStorageService fileStorageService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<VendorRegistrationResponse>> Handle(RegisterVendorCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<VendorRegistrationResponse>.Failure(
                "Unauthorized",
                new List<string> { "User not authenticated" }
            );
        }

        var user = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (user == null)
        {
            return Result<VendorRegistrationResponse>.Failure(
                "User not found",
                new List<string> { "User does not exist" }
            );
        }

        // Check if user already has a vendor account
        var existingVendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.UserId == userId.Value, cancellationToken);
        
        if (existingVendor != null)
        {
            return Result<VendorRegistrationResponse>.Failure(
                "Vendor already exists",
                new List<string> { "You already have a vendor account" }
            );
        }

        // Check if business email is already taken
        var emailExists = await _context.Vendors
            .AnyAsync(v => v.BusinessEmail == request.BusinessEmail, cancellationToken);
        
        if (emailExists)
        {
            return Result<VendorRegistrationResponse>.Failure(
                "Email already registered",
                new List<string> { "This business email is already registered" }
            );
        }

        // Validate documents
        if (request.Documents == null || request.Documents.Count == 0)
        {
            return Result<VendorRegistrationResponse>.Failure(
                "Documents required",
                new List<string> { "At least one verification document is required" }
            );
        }

        // Validate each document
        var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
        var maxFileSize = 5 * 1024 * 1024; // 5MB

        foreach (var doc in request.Documents)
        {
            var fileExtension = Path.GetExtension(doc.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return Result<VendorRegistrationResponse>.Failure(
                    "Invalid file type",
                    new List<string> { $"Document {doc.FileName} must be PDF, JPEG, or PNG" }
                );
            }

            if (doc.FileSize > maxFileSize)
            {
                return Result<VendorRegistrationResponse>.Failure(
                    "File too large",
                    new List<string> { $"Document {doc.FileName} must be less than 5MB" }
                );
            }
        }

        try
        {
            // Create vendor entity
            var vendor = new Domain.Entities.Vendor
            {
                Id = Guid.NewGuid(),
                UserId = userId.Value,
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
                VerificationStatus = VerificationStatus.Pending,
                IsActive = false, // Inactive until approved
                AcceptOrders = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user.Email
            };

            await _context.Vendors.AddAsync(vendor, cancellationToken);

            // Upload and create verification documents
            var uploadedDocuments = new List<VendorVerification>();
            foreach (var doc in request.Documents)
            {
                try
                {
                    var fileName = $"vendor-documents/{vendor.Id}/{doc.DocumentType}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(doc.FileName)}";
                    var filePath = await _fileStorageService.UploadFileAsync(
                        doc.FileStream,
                        fileName,
                        doc.ContentType,
                        cancellationToken);

                    var verification = new VendorVerification
                    {
                        Id = Guid.NewGuid(),
                        VendorId = vendor.Id,
                        DocumentType = (Domain.Entities.DocumentType)doc.DocumentType,
                        FileName = doc.FileName,
                        FilePath = filePath,
                        ContentType = doc.ContentType,
                        FileSize = doc.FileSize,
                        Status = Domain.Entities.DocumentStatus.Pending,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = user.Email
                    };

                    uploadedDocuments.Add(verification);
                }
                catch (Exception ex)
                {
                    // Rollback: delete uploaded files
                    foreach (var uploaded in uploadedDocuments)
                    {
                        try
                        {
                            await _fileStorageService.DeleteFileAsync(uploaded.FilePath, cancellationToken);
                        }
                        catch { }
                    }
                    return Result<VendorRegistrationResponse>.Failure(
                        "Document upload failed",
                        new List<string> { $"Failed to upload {doc.FileName}: {ex.Message}" }
                    );
                }
            }

            await _context.VendorVerifications.AddRangeAsync(uploadedDocuments, cancellationToken);

            // Update user role to Vendor (if not already)
            var isVendor = await _userManager.IsInRoleAsync(user, "Vendor");
            if (!isVendor)
            {
                await _userManager.AddToRoleAsync(user, "Vendor");
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result<VendorRegistrationResponse>.Success(
                new VendorRegistrationResponse
                {
                    VendorId = vendor.Id,
                    BusinessName = vendor.BusinessName,
                    VerificationStatus = VerificationStatus.Pending
                },
                "Vendor registration submitted successfully. Your account will be reviewed."
            );
        }
        catch (Exception ex)
        {
            return Result<VendorRegistrationResponse>.Failure(
                "Registration failed",
                new List<string> { ex.Message }
            );
        }
    }
}
