using Airbnb.BookingService.Domain;
using Airbnb.SharedKernel.Domain;
using Airbnb.SharedKernel.Events;
using Airbnb.SharedKernel.Infrastructure;

namespace Airbnb.BookingService.Infrastructure.Messaging;

public class BookingIntegrationEventMapper : IIntegrationEventMapper
{
    public object Map(IDomainEvent domainEvent) => domainEvent switch
    {
        BookingCreatedDomainEvent e => new BookingCreatedEvent(
            e.BookingId, 
            e.PropertyId, 
            e.GuestId, 
            e.TotalPrice, 
            e.CurrencyCode, 
            e.CountryCode),

        BookingConfirmedDomainEvent e => new BookingConfirmedEvent(
            e.BookingId, 
            Guid.Empty, // TODO: PropertyId if needed by external systems
            Guid.Empty, // TODO: UserId
            0,          // TODO: TotalPrice
            DateTimeOffset.MinValue, 
            DateTimeOffset.MinValue),

        BookingCancelledDomainEvent e => new BookingCancelledEvent(
            e.BookingId, 
            Guid.Empty, 
            "Booking cancelled by system or host"),

        _ => throw new ArgumentException($"Unhandled domain event type: {domainEvent.GetType().Name}", nameof(domainEvent))
    };
}
