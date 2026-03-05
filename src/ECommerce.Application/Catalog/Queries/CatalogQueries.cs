using MediatR;

namespace ECommerce.Application.Catalog.Queries;

// --- DTOs ---
public record ProductDto(
    Guid Id, string Name, string? Description, string? ImageUrl,
    decimal PriceAmount, string PriceCurrency,
    int StockQuantity, bool IsActive,
    Guid CategoryId, string? CategoryName);

public record CategoryDto(Guid Id, string Name, string? Description, string? ImageUrl);

// --- Queries ---
public record GetProductsQuery(Guid? CategoryId = null) : IRequest<IReadOnlyList<ProductDto>>;
public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;
public record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;
