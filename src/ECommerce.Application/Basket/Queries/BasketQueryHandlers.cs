using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Basket.Queries;

public class GetBasketHandler : IRequestHandler<GetBasketQuery, BasketDto?>
{
    private readonly IApplicationDbContext _db;
    public GetBasketHandler(IApplicationDbContext db) => _db = db;

    public async Task<BasketDto?> Handle(GetBasketQuery q, CancellationToken ct)
    {
        var query = _db.Baskets.AsNoTracking().Include(b => b.Items).AsQueryable();
        var basket = q.UserId.HasValue
            ? await query.FirstOrDefaultAsync(b => b.UserId == q.UserId, ct)
            : await query.FirstOrDefaultAsync(b => b.SessionId == q.SessionId, ct);
        if (basket is null) return null;
        return new BasketDto(basket.Id,
            basket.Items.Select(i => new BasketItemDto(i.ProductId, i.ProductName,
                i.UnitPrice.Amount, i.Quantity, i.TotalPrice.Amount)).ToList(),
            basket.Total.Amount, basket.Total.Currency);
    }
}
