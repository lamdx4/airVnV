using MassTransit;
using Airbnb.SharedKernel.Events;

namespace Airbnb.BookingService.Infrastructure.Saga;

public class BookingStateMachine : MassTransitStateMachine<BookingState>
{
    public BookingStateMachine()
    {
        InstanceState(x => x.CurrentState);

        // Define Correlations
        Event(() => BookingCreated, x => x.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentSucceeded, x => x.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentFailed, x => x.CorrelateById(m => m.Message.BookingId));
        Event(() => BookingConfirmed, x => x.CorrelateById(m => m.Message.BookingId));
        Event(() => BookingCancelled, x => x.CorrelateById(m => m.Message.BookingId));

        // Schedule for 15-minute timeout
        Schedule(() => PaymentTimeout, x => x.ExpirationTokenId, x =>
        {
            x.Delay = TimeSpan.FromMinutes(15);
            x.Received = e => e.CorrelateById(context => context.Message.BookingId);
        });

        // Schedule for 24-hour host approval timeout
        Schedule(() => ApprovalTimeout, x => x.ExpirationTokenId, x =>
        {
            x.Delay = TimeSpan.FromHours(24);
            x.Received = e => e.CorrelateById(context => context.Message.BookingId);
        });

        Initially(
            When(BookingCreated)
                .Then(context =>
                {
                    context.Saga.BookingId = context.Message.BookingId;
                    context.Saga.PropertyId = context.Message.PropertyId;
                    context.Saga.GuestId = context.Message.GuestId;
                    context.Saga.TotalPrice = context.Message.TotalPrice;
                    context.Saga.CurrencyCode = context.Message.CurrencyCode;
                    context.Saga.BookingMode = context.Message.BookingMode;
                    context.Saga.CreatedAt = DateTimeOffset.UtcNow;
                })
                .Send(context => new InitiatePaymentCommand(
                    context.Saga.BookingId,
                    context.Saga.TotalPrice,
                    context.Saga.CurrencyCode,
                    context.Message.CountryCode,
                    context.Saga.GuestId))
                .Schedule(PaymentTimeout, context => new PaymentTimeoutEvent(context.Saga.BookingId))
                .TransitionTo(AwaitingPayment)
        );

        During(AwaitingPayment,
            When(PaymentSucceeded)
                .Unschedule(PaymentTimeout)
                .IfElse(context => context.Saga.BookingMode == Airbnb.BookingService.Domain.Enums.BookingMode.InstantBook,
                    x => x.TransitionTo(Confirmed),
                    x => x.Schedule(ApprovalTimeout, context => new BookingApprovalTimeoutEvent(context.Saga.BookingId))
                          .TransitionTo(AwaitingHostApproval)),

            When(PaymentFailed)
                .Unschedule(PaymentTimeout)
                .Publish(context => new BookingCancelledEvent(
                    context.Saga.BookingId,
                    context.Saga.PropertyId,
                    $"Payment failed: {context.Message.ErrorCode}"))
                .TransitionTo(Cancelled),

            When(PaymentTimeout.AnyReceived)
                .Publish(context => new BookingCancelledEvent(
                    context.Saga.BookingId,
                    context.Saga.PropertyId,
                    "Payment timeout (15 minutes exceeded)"))
                .TransitionTo(Cancelled)
        );

        During(AwaitingHostApproval,
            When(BookingConfirmed)
                .Unschedule(ApprovalTimeout)
                .TransitionTo(Confirmed),

            When(BookingCancelled)
                .Unschedule(ApprovalTimeout)
                .Send(context => new RefundPaymentCommand(context.Saga.BookingId, context.Message.Reason))
                .TransitionTo(Cancelled),

            When(ApprovalTimeout.AnyReceived)
                // Saga tự bắn event để Consumer gọi DB Hủy, đồng thời gửi lệnh hoàn tiền luôn
                .Publish(context => new BookingCancelledEvent(
                    context.Saga.BookingId,
                    context.Saga.PropertyId,
                    "Host approval timeout (24 hours exceeded)"))
                .Send(context => new RefundPaymentCommand(context.Saga.BookingId, "Host approval timeout (24 hours exceeded)"))
                .TransitionTo(Cancelled)
        );
    }

    // States
    public State AwaitingPayment { get; private set; } = default!;
    public State AwaitingHostApproval { get; private set; } = default!;
    public State Confirmed { get; private set; } = default!;
    public State Cancelled { get; private set; } = default!;

    // Events
    public Event<BookingCreatedEvent> BookingCreated { get; private set; } = default!;
    public Event<PaymentSucceededEvent> PaymentSucceeded { get; private set; } = default!;
    public Event<PaymentFailedEvent> PaymentFailed { get; private set; } = default!;
    public Event<BookingConfirmedEvent> BookingConfirmed { get; private set; } = default!;
    public Event<BookingCancelledEvent> BookingCancelled { get; private set; } = default!;

    // Schedules
    public Schedule<BookingState, PaymentTimeoutEvent> PaymentTimeout { get; private set; } = default!;
    public Schedule<BookingState, BookingApprovalTimeoutEvent> ApprovalTimeout { get; private set; } = default!;
}

public record PaymentTimeoutEvent(Guid BookingId);
public record BookingApprovalTimeoutEvent(Guid BookingId);
