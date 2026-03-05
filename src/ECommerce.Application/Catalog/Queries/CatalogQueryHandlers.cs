using ECommerce.Domain.Catalog;
using ECommerce.Domain.Catalog.Entities;
using MediatR;

namespace ECommerce.Application.Catalog.Queries;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly IProductRepository _products;
    public GetProductsHandler(IProductRepository products) => _products = products;

    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery q, CancellationToken ct)
    {
        var products = q.CategoryId.HasValue
            ? await _products.GetByCategoryAsync(q.CategoryId.Value, ct)
            : await _products.GetAllActiveAsync(ct);

        return products.Select(p => new ProductDto(
            p.Id, p.Name, p.Description, p.ImageUrl,
            p.Price.Amount, p.Price.Currency, p.Stock.Value, p.IsActive,
            p.CategoryId, p.Category!.Name)).ToList();
    }
}

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _products;
    public GetProductByIdHandler(IProductRepository products) => _products = products;

    public async Task<ProductDto?> Handle(GetProductByIdQuery q, CancellationToken ct)
    {
        var products = await _products.GetAllActiveAsync(ct);
        var p = products.FirstOrDefault(x => x.Id == q.Id);
        if (p is null) return null;
        return new ProductDto(p.Id, p.Name, p.Description, p.ImageUrl,
            p.Price.Amount, p.Price.Currency, p.Stock.Value, p.IsActive,
            p.CategoryId, p.Category!.Name);
    }
}

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly ICategoryRepository _categories;
    public GetCategoriesHandler(ICategoryRepository categories) => _categories = categories;

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery q, CancellationToken ct)
    {
        var categories = await _categories.GetAllAsync(ct);
        return categories.Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.ImageUrl)).ToList();
    }
}
