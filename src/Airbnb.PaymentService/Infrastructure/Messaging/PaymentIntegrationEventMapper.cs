using Airbnb.PaymentService.Domain;
using Airbnb.SharedKernel.Domain;
using Airbnb.SharedKernel.Events;
using Airbnb.SharedKernel.Infrastructure;

namespace Airbnb.PaymentService.Infrastructure.Messaging;

public class PaymentIntegrationEventMapper : IIntegrationEventMapper
{
    public object Map(IDomainEvent domainEvent) => domainEvent switch
    {
        PaymentSucceededDomainEvent e => new PaymentSucceededEvent(
            e.PaymentId, 
            e.BookingId, 
            e.Amount, 
            e.Currency, 
            e.TransactionId),

        PaymentFailedDomainEvent e => new PaymentFailedEvent(
            e.PaymentId, 
            e.BookingId, 
            e.ErrorCode),

        _ => throw new ArgumentException($"Unhandled domain event type: {domainEvent.GetType().Name}")
    };
}
