using Airbnb.BookingService.Domain;
using Airbnb.SharedKernel.Domain;
using Airbnb.SharedKernel.Infrastructure;
using Mediator;

namespace Airbnb.BookingService.Infrastructure.Messaging;

public class BookingDomainEventPolicyExecutor(IMediator mediator) : IDomainEventPolicyExecutor
{
    public async Task ExecuteAsync(IEnumerable<IDomainEvent> events, CancellationToken ct)
    {
        foreach (var @event in events)
        {
            var notification = MapToNotification(@event);
            await mediator.Publish(notification, ct);
        }
    }

    private INotification MapToNotification(IDomainEvent @event) => @event switch
    {
        BookingCreatedDomainEvent e           => new BookingCreatedNotification(e),
        BookingConfirmedDomainEvent e         => new BookingConfirmedNotification(e),
        BookingCancelledDomainEvent e         => new BookingCancelledNotification(e),
        // Bug #5 Fix: missing case would throw ArgumentException → SaveChangesAsync fails after AwaitForApproval()
        BookingAwaitingApprovalDomainEvent e  => new BookingAwaitingApprovalNotification(e),
        BookingRefundingDomainEvent e          => new BookingRefundingNotification(e),
        _ => throw new ArgumentException($"Unhandled domain event type: {@event.GetType().Name}")
    };
}
