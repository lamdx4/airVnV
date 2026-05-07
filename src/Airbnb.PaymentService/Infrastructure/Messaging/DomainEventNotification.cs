using Airbnb.PaymentService.Domain;
using Mediator;

namespace Airbnb.PaymentService.Infrastructure.Messaging;

public record PaymentInitiatedNotification(PaymentInitiatedDomainEvent DomainEvent) : INotification;
public record PaymentSucceededNotification(PaymentSucceededDomainEvent DomainEvent) : INotification;
public record PaymentFailedNotification(PaymentFailedDomainEvent DomainEvent) : INotification;
