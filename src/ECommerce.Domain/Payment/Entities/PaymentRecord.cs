using ECommerce.Domain.Catalog.ValueObjects;
using ECommerce.Domain.Common;

namespace ECommerce.Domain.Payment.Entities;

public enum PaymentStatus { Pending, Succeeded, Failed, Refunded }

public class PaymentRecord : BaseAuditableEntity
{
    public Guid OrderId { get; private set; }
    public Money Amount { get; private set; } = default!;
    public string Provider { get; private set; } = default!;     // e.g. "Stripe", "Iyzico"
    public string? ProviderTransactionId { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? FailureReason { get; private set; }

    private PaymentRecord() { }

    public static PaymentRecord CreatePending(Guid orderId, Money amount, string provider)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(provider);
        return new PaymentRecord
        {
            OrderId = orderId,
            Amount = amount,
            Provider = provider,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkSucceeded(string providerTransactionId)
    {
        Status = PaymentStatus.Succeeded;
        ProviderTransactionId = providerTransactionId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string reason)
    {
        Status = PaymentStatus.Failed;
        FailureReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }
}
