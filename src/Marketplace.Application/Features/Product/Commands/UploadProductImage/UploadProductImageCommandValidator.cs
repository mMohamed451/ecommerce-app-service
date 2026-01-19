using FluentValidation;

namespace Marketplace.Application.Features.Product.Commands.UploadProductImage;

public class UploadProductImageCommandValidator : AbstractValidator<UploadProductImageCommand>
{
    public UploadProductImageCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.FileStream)
            .NotNull().WithMessage("File stream is required");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required")
            .MaximumLength(255).WithMessage("File name must not exceed 255 characters");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Content type is required")
            .Must(BeValidImageType).WithMessage("File must be a valid image type (JPEG, PNG, GIF, WebP)");

        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage("File size must be greater than 0")
            .LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("File size must not exceed 10MB");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative");

        RuleFor(x => x.AltText)
            .MaximumLength(200).WithMessage("Alt text must not exceed 200 characters");
    }

    private bool BeValidImageType(string contentType)
    {
        var validTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
        return validTypes.Contains(contentType.ToLowerInvariant());
    }
}
