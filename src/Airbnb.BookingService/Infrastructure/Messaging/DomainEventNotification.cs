using Airbnb.BookingService.Domain;
using Mediator;

namespace Airbnb.BookingService.Infrastructure.Messaging;

public record BookingCreatedNotification(BookingCreatedDomainEvent DomainEvent) : INotification;
public record BookingConfirmedNotification(BookingConfirmedDomainEvent DomainEvent) : INotification;
public record BookingCancelledNotification(BookingCancelledDomainEvent DomainEvent) : INotification;
// Bug #5 Fix: must exist before PolicyExecutor and Mapper can reference it
public record BookingAwaitingApprovalNotification(BookingAwaitingApprovalDomainEvent DomainEvent) : INotification;
