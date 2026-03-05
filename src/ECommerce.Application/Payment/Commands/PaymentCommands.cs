using MediatR;

namespace ECommerce.Application.Payment.Commands;

public record ProcessPaymentCommand(
    Guid OrderId,
    string PaymentToken) : IRequest<Guid>;  // Returns PaymentRecord Id
