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

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    // Required for timeout scheduling
    public Guid? ExpirationTokenId { get; set; }
}
