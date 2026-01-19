using FluentValidation;

namespace Marketplace.Application.Features.Product.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters");

        RuleFor(x => x.ShortDescription)
            .MaximumLength(500).WithMessage("Short description must not exceed 500 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.CompareAtPrice)
            .GreaterThan(x => x.Price).When(x => x.CompareAtPrice.HasValue)
            .WithMessage("Compare at price must be greater than regular price");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative");

        RuleFor(x => x.LowStockThreshold)
            .GreaterThanOrEqualTo(0).When(x => x.LowStockThreshold.HasValue)
            .WithMessage("Low stock threshold cannot be negative");

        RuleFor(x => x.Weight)
            .GreaterThan(0).When(x => x.Weight.HasValue && x.RequiresShipping)
            .WithMessage("Weight must be greater than 0 for products requiring shipping");

        RuleFor(x => x.SKU)
            .MaximumLength(100).WithMessage("SKU must not exceed 100 characters");

        RuleFor(x => x.Barcode)
            .MaximumLength(100).WithMessage("Barcode must not exceed 100 characters");

        RuleForEach(x => x.Attributes)
            .SetValidator(new ProductAttributeInputValidator());

        RuleForEach(x => x.Variations)
            .SetValidator(new ProductVariationInputValidator());
    }
}

public class ProductAttributeInputValidator : AbstractValidator<ProductAttributeInput>
{
    public ProductAttributeInputValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Attribute name is required")
            .MaximumLength(100).WithMessage("Attribute name must not exceed 100 characters");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Attribute value is required")
            .MaximumLength(500).WithMessage("Attribute value must not exceed 500 characters");
    }
}

public class ProductVariationInputValidator : AbstractValidator<ProductVariationInput>
{
    public ProductVariationInputValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Variation name is required")
            .MaximumLength(200).WithMessage("Variation name must not exceed 200 characters");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative");

        RuleFor(x => x.Price)
            .GreaterThan(0).When(x => x.Price.HasValue)
            .WithMessage("Variation price must be greater than 0");

        RuleForEach(x => x.Attributes)
            .SetValidator(new ProductVariationAttributeInputValidator());
    }
}

public class ProductVariationAttributeInputValidator : AbstractValidator<ProductVariationAttributeInput>
{
    public ProductVariationAttributeInputValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Variation attribute name is required")
            .MaximumLength(100).WithMessage("Variation attribute name must not exceed 100 characters");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Variation attribute value is required")
            .MaximumLength(200).WithMessage("Variation attribute value must not exceed 200 characters");
    }
}
