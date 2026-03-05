using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Ordering.Queries;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, IReadOnlyList<OrderDto>>
{
    private readonly IApplicationDbContext _db;
    public GetOrdersHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<OrderDto>> Handle(GetOrdersQuery q, CancellationToken ct)
    {
        var query = _db.Orders.AsNoTracking().Include(o => o.Items).AsQueryable();
        if (q.UserId.HasValue) query = query.Where(o => o.UserId == q.UserId);
        return await query.OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderDto(o.Id, o.OrderNumber, o.Status.ToString(),
                o.Total.Amount, o.Total.Currency, o.ShippingAddress, o.CreatedAt,
                o.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName,
                    i.UnitPrice.Amount, i.Quantity, i.LineTotal.Amount)).ToList()))
            .ToListAsync(ct);
    }
}

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IApplicationDbContext _db;
    public GetOrderByIdHandler(IApplicationDbContext db) => _db = db;

    public async Task<OrderDto?> Handle(GetOrderByIdQuery q, CancellationToken ct)
    {
        return await _db.Orders.AsNoTracking().Include(o => o.Items).Where(o => o.Id == q.Id)
            .Select(o => new OrderDto(o.Id, o.OrderNumber, o.Status.ToString(),
                o.Total.Amount, o.Total.Currency, o.ShippingAddress, o.CreatedAt,
                o.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName,
                    i.UnitPrice.Amount, i.Quantity, i.LineTotal.Amount)).ToList()))
            .FirstOrDefaultAsync(ct);
    }
}
