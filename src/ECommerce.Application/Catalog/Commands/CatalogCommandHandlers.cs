using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Catalog.Entities;
using ECommerce.Domain.Catalog.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Catalog.Commands;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    public CreateProductHandler(IApplicationDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateProductCommand cmd, CancellationToken ct)
    {
        var product = Product.Create(cmd.Name, cmd.Description, cmd.ImageUrl,
            new Money(cmd.Price, cmd.Currency), new StockQuantity(cmd.StockQuantity), cmd.CategoryId);
        await _db.Products.AddAsync(product, ct);
        await _db.SaveChangesAsync(ct);
        return product.Id;
    }
}

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IApplicationDbContext _db;
    public UpdateProductHandler(IApplicationDbContext db) => _db = db;

    public async Task Handle(UpdateProductCommand cmd, CancellationToken ct)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == cmd.Id, ct)
            ?? throw new KeyNotFoundException($"Product {cmd.Id} not found.");
        product.UpdateDetails(cmd.Name, cmd.Description, cmd.ImageUrl,
            new Money(cmd.Price, cmd.Currency), cmd.CategoryId);
        await _db.SaveChangesAsync(ct);
    }
}

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IApplicationDbContext _db;
    public DeleteProductHandler(IApplicationDbContext db) => _db = db;

    public async Task Handle(DeleteProductCommand cmd, CancellationToken ct)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == cmd.Id, ct)
            ?? throw new KeyNotFoundException($"Product {cmd.Id} not found.");
        _db.Products.Remove(product);
        await _db.SaveChangesAsync(ct);
    }
}

public class UpdateStockHandler : IRequestHandler<UpdateStockCommand>
{
    private readonly IApplicationDbContext _db;
    public UpdateStockHandler(IApplicationDbContext db) => _db = db;

    public async Task Handle(UpdateStockCommand cmd, CancellationToken ct)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == cmd.ProductId, ct)
            ?? throw new KeyNotFoundException($"Product {cmd.ProductId} not found.");
        product.UpdateStock(cmd.NewQuantity);
        await _db.SaveChangesAsync(ct);
    }
}

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    public CreateCategoryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateCategoryCommand cmd, CancellationToken ct)
    {
        var category = Category.Create(cmd.Name, cmd.Description, cmd.ImageUrl);
        await _db.Categories.AddAsync(category, ct);
        await _db.SaveChangesAsync(ct);
        return category.Id;
    }
}
