using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Ordering.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Ordering.Commands;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    public CreateOrderHandler(IApplicationDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateOrderCommand cmd, CancellationToken ct)
    {
        var basket = cmd.UserId.HasValue
            ? await _db.Baskets.Include(b => b.Items).FirstOrDefaultAsync(b => b.UserId == cmd.UserId, ct)
            : await _db.Baskets.Include(b => b.Items).FirstOrDefaultAsync(b => b.SessionId == cmd.SessionId, ct);

        if (basket is null || !basket.Items.Any())
            throw new InvalidOperationException("Basket is empty.");

        var orderItems = basket.Items.Select(i =>
            OrderItem.Create(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity));

        var order = Order.Create(cmd.UserId, cmd.GuestEmail, cmd.ShippingAddress, orderItems);

        foreach (var item in basket.Items)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, ct)
                ?? throw new KeyNotFoundException($"Product {item.ProductId} not found.");
            product.DecreaseStock(item.Quantity);
        }

        await _db.Orders.AddAsync(order, ct);
        basket.Clear();
        await _db.SaveChangesAsync(ct);
        return order.Id;
    }
}
