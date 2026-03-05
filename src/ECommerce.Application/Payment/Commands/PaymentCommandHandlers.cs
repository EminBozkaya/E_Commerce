using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Payment.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Payment.Commands;

public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    private readonly IPaymentService _paymentService;
    public ProcessPaymentHandler(IApplicationDbContext db, IPaymentService paymentService)
    { _db = db; _paymentService = paymentService; }

    public async Task<Guid> Handle(ProcessPaymentCommand cmd, CancellationToken ct)
    {
        var order = await _db.Orders.Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == cmd.OrderId, ct)
            ?? throw new KeyNotFoundException("Order not found.");

        var payment = PaymentRecord.CreatePending(order.Id, order.Total, "CreditCard");
        await _db.PaymentRecords.AddAsync(payment, ct);

        try
        {
            var transactionId = await _paymentService.ChargeAsync(order, cmd.PaymentToken, ct);
            payment.MarkSucceeded(transactionId);
            order.MarkAsPaid();
        }
        catch (Exception ex)
        {
            payment.MarkFailed(ex.Message);
            throw;
        }

        await _db.SaveChangesAsync(ct);
        return payment.Id;
    }
}
