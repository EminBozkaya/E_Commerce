using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Catalog.Queries;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly IApplicationDbContext _db;
    public GetProductsHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery q, CancellationToken ct)
    {
        var query = _db.Products.AsNoTracking().Include(p => p.Category).Where(p => p.IsActive).AsQueryable();
        if (q.CategoryId.HasValue) query = query.Where(p => p.CategoryId == q.CategoryId.Value);
        return await query.Select(p => new ProductDto(
            p.Id, p.Name, p.Description, p.ImageUrl,
            p.Price.Amount, p.Price.Currency, p.Stock.Value, p.IsActive,
            p.CategoryId, p.Category!.Name)).ToListAsync(ct);
    }
}

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IApplicationDbContext _db;
    public GetProductByIdHandler(IApplicationDbContext db) => _db = db;

    public async Task<ProductDto?> Handle(GetProductByIdQuery q, CancellationToken ct)
    {
        return await _db.Products.AsNoTracking().Include(p => p.Category).Where(p => p.Id == q.Id)
            .Select(p => new ProductDto(p.Id, p.Name, p.Description, p.ImageUrl,
                p.Price.Amount, p.Price.Currency, p.Stock.Value, p.IsActive,
                p.CategoryId, p.Category!.Name)).FirstOrDefaultAsync(ct);
    }
}

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly IApplicationDbContext _db;
    public GetCategoriesHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery q, CancellationToken ct)
    {
        return await _db.Categories.AsNoTracking()
            .Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.ImageUrl)).ToListAsync(ct);
    }
}
