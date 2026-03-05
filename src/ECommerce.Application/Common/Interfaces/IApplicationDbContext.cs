using ECommerce.Domain.Basket.Entities;
using ECommerce.Domain.Catalog.Entities;
using ECommerce.Domain.Identity.Entities;
using ECommerce.Domain.Ordering.Entities;
using ECommerce.Domain.Payment.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Common.Interfaces;

/// <summary>
/// Abstraction for the database context.
/// Application layer depends on this interface, NOT on the concrete EF DbContext.
/// architecture-rules §3: Application MUST NOT be aware of DB details.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    DbSet<Domain.Basket.Entities.Basket> Baskets { get; }
    DbSet<BasketItem> BasketItems { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<PaymentRecord> PaymentRecords { get; }
    DbSet<AppUser> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
