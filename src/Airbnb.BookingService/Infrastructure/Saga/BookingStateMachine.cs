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
            x.Delay = TimeSpan.FromMinutes(1);
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
            // NHÁNH 1: InstantBook → FE gọi InitiatePayment trực tiếp, Saga chỉ track state
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

            When(PaymentTimeout.Received)
                .Publish(ctx => new BookingCancelledEvent(
                    ctx.Saga.BookingId,
                    ctx.Saga.PropertyId,
                    "Payment timeout (5 minutes exceeded)"))
                .TransitionTo(Cancelled),

            // Guest hủy trong khi đang chờ thanh toán
            When(BookingCancelled)
                .Unschedule(PaymentTimeout)
                .TransitionTo(Cancelled)
        );

        // ── AwaitingHostApproval ──────────────────────────────────────────────
        // Bug #1 Fix: Host approve → trigger payment flow (no money collected yet)
        // Bug #6 Fix: Removed RefundPaymentCommand — no payment exists at this stage
        During(AwaitingHostApproval,
            // Host duyệt → domain raises BookingAwaitingApprovalEvent → Saga kéo tiền
            When(BookingAwaitingApproval)
                .Unschedule(ApprovalTimeout)
                .Publish(ctx => new InitiatePaymentCommand(
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
            When(ApprovalTimeout.Received)
                .Publish(ctx => new BookingCancelledEvent(
                    ctx.Saga.BookingId,
                    ctx.Saga.PropertyId,
                    "Host approval timeout (24 hours exceeded)"))
                .TransitionTo(Cancelled),

            // Defensive ignores: events that should not arrive here
            Ignore(PaymentSucceeded),
            Ignore(PaymentFailed),
            Ignore(BookingRefunding)
        );

        // ── Confirmed ─────────────────────────────────────────────────────────
        // When a confirmed booking receives a cancellation request that requires a refund,
        // the domain raises BookingRefundingEvent. The Saga orchestrates the refund via
        // RefundPaymentCommand and waits for the outcome before finalizing state.
        During(Confirmed,
            When(BookingRefunding)
                .Publish(ctx => new RefundPaymentCommand(ctx.Saga.BookingId, ctx.Message.Reason))
                .TransitionTo(Refunding),

            Ignore(BookingConfirmed),       // Duplicate domain event after transition
            Ignore(BookingAwaitingApproval) // Stale event from RequestToBook flow
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
                .TransitionTo(RefundFailed),

            // Domain may have already set Booking.Status = Cancelled before Saga transitions.
            // The Saga is authoritative for orchestration — just track and ignore domain echoes.
            Ignore(BookingCancelled),      // Domain cancel event while refund is pending
            Ignore(BookingRefunding),      // Duplicate refund trigger
            Ignore(PaymentSucceeded)       // Stale payment event from previous flow
        );

        // ── RefundFailed ──────────────────────────────────────────────────────
        // Terminal state requiring Admin intervention. Booking is NOT cancelled.
        // The room stays locked until Admin manually resolves the refund.
        During(RefundFailed,
            Ignore(RefundPaymentFailed), // Idempotency: duplicate failure events
            Ignore(BookingCancelled),    // Domain may echo cancel; Saga stays in RefundFailed
            Ignore(PaymentRefunded),     // Late refund event after failure recorded
            Ignore(BookingRefunding)     // Duplicate trigger
        );

        // ── Idempotency: Ignore stale/duplicate events in terminal states ─────
        // Stale messages can arrive from previous sessions or after retries.
        // MassTransit throws UnhandledEventException if no handler exists →
        // causes infinite retry loop. Ignore them explicitly to ACK and discard.
        During(Cancelled,
            Ignore(PaymentSucceeded),
            Ignore(PaymentFailed),
            Ignore(PaymentRefunded),
            Ignore(RefundPaymentFailed),
            Ignore(BookingRefunding),
            Ignore(BookingCancelled),
            Ignore(BookingConfirmed),      // Fix #2: was missing → UnhandledEventException
            Ignore(BookingAwaitingApproval)
        );

        During(AwaitingPayment,
            Ignore(BookingRefunding),      // Cannot refund before payment completes
            Ignore(BookingAwaitingApproval) // Stale approval event from RequestToBook flow
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
