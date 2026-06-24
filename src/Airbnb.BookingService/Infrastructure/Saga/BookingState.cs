using MassTransit;

namespace Airbnb.BookingService.Infrastructure.Saga;

public class BookingState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = default!;

    public Guid BookingId { get; set; }
    public Guid GuestId { get; set; }
    public Guid PropertyId { get; set; }
    
    public decimal TotalPrice { get; set; }
    public string CurrencyCode { get; set; } = default!;
    public string CountryCode { get; set; } = default!;  // Added: needed to route InitiatePaymentCommand after host approval
    public string BookingMode { get; set; } = default!;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    // Bug #3 Fix: Split into 2 separate token fields to avoid mutual overwrite
    public Guid? PaymentTimeoutTokenId { get; set; }    // 15-minute payment timeout
    public Guid? ApprovalTimeoutTokenId { get; set; }   // 24-hour host approval timeout
}
