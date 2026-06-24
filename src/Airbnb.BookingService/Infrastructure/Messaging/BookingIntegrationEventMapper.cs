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
            e.CountryCode,
            e.BookingMode),

        BookingConfirmedDomainEvent e => new BookingConfirmedEvent(
            e.BookingId, 
            e.PropertyId,
            e.GuestId,
            e.TotalPrice,
            new DateTimeOffset(e.CheckIn.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)), 
            new DateTimeOffset(e.CheckOut.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc))),

        // Bug #7 Fix: use actual PropertyId and Reason from domain event (no more hardcoded Guid.Empty)
        BookingCancelledDomainEvent e => new BookingCancelledEvent(
            e.BookingId, 
            e.PropertyId,
            e.Reason),

        // Bug #5 Fix: missing case would throw ArgumentException → Outbox publish fails
        BookingAwaitingApprovalDomainEvent e => new BookingAwaitingApprovalEvent(
            e.BookingId,
            e.PropertyId,
            e.HostId,
            e.GuestId),

        BookingRefundingDomainEvent e => new BookingRefundingEvent(
            e.BookingId,
            e.PropertyId,
            e.Reason),

        _ => throw new ArgumentException($"Unhandled domain event type: {domainEvent.GetType().Name}", nameof(domainEvent))
    };
}
