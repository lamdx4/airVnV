namespace Airbnb.SharedKernel.Events;

public record BookingConfirmedEvent(Guid BookingId, Guid PropertyId, Guid UserId, decimal TotalPrice, DateTimeOffset CheckIn, DateTimeOffset CheckOut)
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
