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
            e.PropertyId,
            e.GuestId,
            e.TotalPrice,
            new DateTimeOffset(e.CheckIn.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)), 
            new DateTimeOffset(e.CheckOut.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc))),

        BookingCancelledDomainEvent e => new BookingCancelledEvent(
            e.BookingId, 
            Guid.Empty, 
            "Booking cancelled by system or host"),

        _ => throw new ArgumentException($"Unhandled domain event type: {domainEvent.GetType().Name}", nameof(domainEvent))
    };
}
