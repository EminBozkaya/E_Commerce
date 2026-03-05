using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Basket.Commands;

public class AddToBasketHandler : IRequestHandler<AddToBasketCommand>
{
    private readonly IApplicationDbContext _db;
    public AddToBasketHandler(IApplicationDbContext db) => _db = db;

    public async Task Handle(AddToBasketCommand cmd, CancellationToken ct)
    {
        var product = await _db.Products.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == cmd.ProductId, ct)
            ?? throw new KeyNotFoundException("Product not found.");

        var basket = await GetOrCreateBasket(cmd.UserId, cmd.SessionId, ct);
        basket.AddItem(product.Id, product.Name, product.Price, cmd.Quantity);
        await _db.SaveChangesAsync(ct);
    }

    private async Task<Domain.Basket.Entities.Basket> GetOrCreateBasket(Guid? userId, string? sessionId, CancellationToken ct)
    {
        Domain.Basket.Entities.Basket? basket;
        if (userId.HasValue)
        {
            basket = await _db.Baskets.Include(b => b.Items).FirstOrDefaultAsync(b => b.UserId == userId, ct);
            if (basket is null) { basket = Domain.Basket.Entities.Basket.CreateForUser(userId.Value); await _db.Baskets.AddAsync(basket, ct); }
        }
        else
        {
            basket = await _db.Baskets.Include(b => b.Items).FirstOrDefaultAsync(b => b.SessionId == sessionId, ct);
            if (basket is null) { basket = Domain.Basket.Entities.Basket.CreateForGuest(sessionId!); await _db.Baskets.AddAsync(basket, ct); }
        }
        return basket;
    }
}

public class RemoveFromBasketHandler : IRequestHandler<RemoveFromBasketCommand>
{
    private readonly IApplicationDbContext _db;
    public RemoveFromBasketHandler(IApplicationDbContext db) => _db = db;

    public async Task Handle(RemoveFromBasketCommand cmd, CancellationToken ct)
    {
        var basket = cmd.UserId.HasValue
            ? await _db.Baskets.Include(b => b.Items).FirstOrDefaultAsync(b => b.UserId == cmd.UserId, ct)
            : await _db.Baskets.Include(b => b.Items).FirstOrDefaultAsync(b => b.SessionId == cmd.SessionId, ct);
        if (basket is null) return;
        basket.RemoveItem(cmd.ProductId);
        await _db.SaveChangesAsync(ct);
    }
}

public class ClearBasketHandler : IRequestHandler<ClearBasketCommand>
{
    private readonly IApplicationDbContext _db;
    public ClearBasketHandler(IApplicationDbContext db) => _db = db;

    public async Task Handle(ClearBasketCommand cmd, CancellationToken ct)
    {
        var basket = cmd.UserId.HasValue
            ? await _db.Baskets.Include(b => b.Items).FirstOrDefaultAsync(b => b.UserId == cmd.UserId, ct)
            : await _db.Baskets.Include(b => b.Items).FirstOrDefaultAsync(b => b.SessionId == cmd.SessionId, ct);
        if (basket is null) return;
        basket.Clear();
        await _db.SaveChangesAsync(ct);
    }
}
