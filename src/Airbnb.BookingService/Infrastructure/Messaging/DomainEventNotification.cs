using Airbnb.BookingService.Domain;
using Mediator;

namespace Airbnb.BookingService.Infrastructure.Messaging;

public record BookingCreatedNotification(BookingCreatedDomainEvent DomainEvent) : INotification;
public record BookingConfirmedNotification(BookingConfirmedDomainEvent DomainEvent) : INotification;
public record BookingCancelledNotification(BookingCancelledDomainEvent DomainEvent) : INotification;
