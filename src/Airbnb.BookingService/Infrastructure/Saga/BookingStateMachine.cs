using Airbnb.BookingService.Domain.Enums;
using Airbnb.SharedKernel.Events;
using MassTransit;

namespace Airbnb.BookingService.Infrastructure.Saga;

public class BookingStateMachine : MassTransitStateMachine<BookingState>
{
    public BookingStateMachine()
    {
        InstanceState(x => x.CurrentState);

        // ── Correlations ─────────────────────────────────────────────────────
        Event(() => BookingCreated,          x => x.CorrelateById(m => m.Message.BookingId));
        Event(() => BookingAwaitingApproval, x => x.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentSucceeded,        x => x.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentFailed,           x => x.CorrelateById(m => m.Message.BookingId));
        Event(() => BookingConfirmed,        x => x.CorrelateById(m => m.Message.BookingId));
        Event(() => BookingCancelled,        x => x.CorrelateById(m => m.Message.BookingId));
        Event(() => BookingRefunding,        x => x.CorrelateById(m => m.Message.BookingId));
        Event(() => PaymentRefunded,         x => x.CorrelateById(m => m.Message.BookingId));
        Event(() => RefundPaymentFailed,     x => x.CorrelateById(m => m.Message.BookingId));

        // ── Schedules ─────────────────────────────────────────────────────────
        // Bug #3 Fix: each schedule maps to its own dedicated token field
        Schedule(() => PaymentTimeout, x => x.PaymentTimeoutTokenId, x =>
        {
            x.Delay = TimeSpan.FromMinutes(15);
            x.Received = e => e.CorrelateById(context => context.Message.BookingId);
        });

        Schedule(() => ApprovalTimeout, x => x.ApprovalTimeoutTokenId, x =>
        {
            x.Delay = TimeSpan.FromHours(24);
            x.Received = e => e.CorrelateById(context => context.Message.BookingId);
        });

        // ── Initially ─────────────────────────────────────────────────────────
        // Bug #1 Fix: split on BookingMode — RequestToBook does NOT initiate payment here
        Initially(
            // NHÁNH 1: InstantBook → kéo tiền ngay
            When(BookingCreated, ctx => ctx.Message.BookingMode == BookingMode.InstantBook)
                .Then(ctx =>
                {
                    ctx.Saga.BookingId    = ctx.Message.BookingId;
                    ctx.Saga.PropertyId   = ctx.Message.PropertyId;
                    ctx.Saga.GuestId      = ctx.Message.GuestId;
                    ctx.Saga.TotalPrice   = ctx.Message.TotalPrice;
                    ctx.Saga.CurrencyCode = ctx.Message.CurrencyCode;
                    ctx.Saga.CountryCode  = ctx.Message.CountryCode;
                    ctx.Saga.BookingMode  = ctx.Message.BookingMode;
                    ctx.Saga.CreatedAt    = DateTimeOffset.UtcNow;
                })
                .Send(ctx => new InitiatePaymentCommand(
                    ctx.Saga.BookingId,
                    ctx.Saga.TotalPrice,
                    ctx.Saga.CurrencyCode,
                    ctx.Saga.CountryCode,
                    ctx.Saga.GuestId))
                .Schedule(PaymentTimeout, ctx => new PaymentTimeoutEvent(ctx.Saga.BookingId))
                .TransitionTo(AwaitingPayment),

            // NHÁNH 2: RequestToBook → chờ Host duyệt TRƯỚC, KHÔNG kéo tiền
            When(BookingCreated, ctx => ctx.Message.BookingMode == BookingMode.RequestToBook)
                .Then(ctx =>
                {
                    ctx.Saga.BookingId    = ctx.Message.BookingId;
                    ctx.Saga.PropertyId   = ctx.Message.PropertyId;
                    ctx.Saga.GuestId      = ctx.Message.GuestId;
                    ctx.Saga.TotalPrice   = ctx.Message.TotalPrice;
                    ctx.Saga.CurrencyCode = ctx.Message.CurrencyCode;
                    ctx.Saga.CountryCode  = ctx.Message.CountryCode;
                    ctx.Saga.BookingMode  = ctx.Message.BookingMode;
                    ctx.Saga.CreatedAt    = DateTimeOffset.UtcNow;
                })
                .Schedule(ApprovalTimeout, ctx => new BookingApprovalTimeoutEvent(ctx.Saga.BookingId))
                .TransitionTo(AwaitingHostApproval)
        );

        // ── AwaitingPayment ───────────────────────────────────────────────────
        During(AwaitingPayment,
            When(PaymentSucceeded)
                .Unschedule(PaymentTimeout)
                .TransitionTo(Confirmed),

            When(PaymentFailed)
                .Unschedule(PaymentTimeout)
                .Publish(ctx => new BookingCancelledEvent(
                    ctx.Saga.BookingId,
                    ctx.Saga.PropertyId,
                    $"Payment failed: {ctx.Message.ErrorCode}"))
                .TransitionTo(Cancelled),

            When(PaymentTimeout.AnyReceived)
                .Publish(ctx => new BookingCancelledEvent(
                    ctx.Saga.BookingId,
                    ctx.Saga.PropertyId,
                    "Payment timeout (15 minutes exceeded)"))
                .TransitionTo(Cancelled)
        );

        // ── AwaitingHostApproval ──────────────────────────────────────────────
        // Bug #1 Fix: Host approve → trigger payment flow (no money collected yet)
        // Bug #6 Fix: Removed RefundPaymentCommand — no payment exists at this stage
        During(AwaitingHostApproval,
            // Host duyệt → domain raises BookingAwaitingApprovalEvent → Saga kéo tiền
            When(BookingAwaitingApproval)
                .Unschedule(ApprovalTimeout)
                .Send(ctx => new InitiatePaymentCommand(
                    ctx.Saga.BookingId,
                    ctx.Saga.TotalPrice,
                    ctx.Saga.CurrencyCode,
                    ctx.Saga.CountryCode,
                    ctx.Saga.GuestId))
                .Schedule(PaymentTimeout, ctx => new PaymentTimeoutEvent(ctx.Saga.BookingId))
                .TransitionTo(AwaitingPayment),

            // Host từ chối → Cancel, KHÔNG refund (tiền chưa thu)
            When(BookingCancelled)
                .Unschedule(ApprovalTimeout)
                .TransitionTo(Cancelled),

            // Timeout 24h → Cancel, KHÔNG refund (tiền chưa thu)
            When(ApprovalTimeout.AnyReceived)
                .Publish(ctx => new BookingCancelledEvent(
                    ctx.Saga.BookingId,
                    ctx.Saga.PropertyId,
                    "Host approval timeout (24 hours exceeded)"))
                .TransitionTo(Cancelled)
        );

        // ── Confirmed ─────────────────────────────────────────────────────────
        // When a confirmed booking receives a cancellation request that requires a refund,
        // the domain raises BookingRefundingEvent. The Saga orchestrates the refund via
        // RefundPaymentCommand and waits for the outcome before finalizing state.
        During(Confirmed,
            When(BookingRefunding)
                .Send(ctx => new RefundPaymentCommand(ctx.Saga.BookingId, ctx.Message.Reason))
                .TransitionTo(Refunding)
        );

        // ── Refunding ─────────────────────────────────────────────────────────
        // Refund is in progress. The room is still locked. Two outcomes:
        // 1. Refund succeeds → transition to Cancelled, publish BookingCancelledEvent.
        // 2. Refund permanently fails → transition to RefundFailed for Admin review.
        During(Refunding,
            When(PaymentRefunded, ctx => ctx.Message.IsFullRefund)
                .Publish(ctx => new BookingCancelledEvent(
                    ctx.Saga.BookingId,
                    ctx.Saga.PropertyId,
                    "Cancelled after successful refund"))
                .TransitionTo(Cancelled),

            When(RefundPaymentFailed)
                .TransitionTo(RefundFailed)
        );

        // ── RefundFailed ──────────────────────────────────────────────────────
        // Terminal state requiring Admin intervention. Booking is NOT cancelled.
        // The room stays locked until Admin manually resolves the refund.
        During(RefundFailed
            // No automatic transitions. Admin must trigger resolution out-of-band.
        );
    }

    // States
    public State AwaitingPayment      { get; private set; } = default!;
    public State AwaitingHostApproval { get; private set; } = default!;
    public State Confirmed            { get; private set; } = default!;
    public State Refunding            { get; private set; } = default!;
    public State RefundFailed         { get; private set; } = default!;
    public State Cancelled            { get; private set; } = default!;

    // Events
    public Event<BookingCreatedEvent>          BookingCreated          { get; private set; } = default!;
    public Event<BookingAwaitingApprovalEvent> BookingAwaitingApproval { get; private set; } = default!;
    public Event<PaymentSucceededEvent>        PaymentSucceeded        { get; private set; } = default!;
    public Event<PaymentFailedEvent>           PaymentFailed           { get; private set; } = default!;
    public Event<BookingConfirmedEvent>        BookingConfirmed        { get; private set; } = default!;
    public Event<BookingCancelledEvent>        BookingCancelled        { get; private set; } = default!;
    public Event<BookingRefundingEvent>        BookingRefunding        { get; private set; } = default!;
    public Event<PaymentRefundedEvent>         PaymentRefunded         { get; private set; } = default!;
    public Event<RefundPaymentFailedEvent>     RefundPaymentFailed     { get; private set; } = default!;

    // Schedules
    public Schedule<BookingState, PaymentTimeoutEvent>         PaymentTimeout  { get; private set; } = default!;
    public Schedule<BookingState, BookingApprovalTimeoutEvent> ApprovalTimeout { get; private set; } = default!;
}

public record PaymentTimeoutEvent(Guid BookingId);
public record BookingApprovalTimeoutEvent(Guid BookingId);
