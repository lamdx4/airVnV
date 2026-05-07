namespace Airbnb.SharedKernel.Events;

public record BookingCreatedEvent(
    Guid BookingId, 
    Guid PropertyId, 
    Guid GuestId, 
    decimal TotalPrice, 
    string CurrencyCode, 
    string CountryCode)
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

public record BookingConfirmedEvent(Guid BookingId, Guid PropertyId, Guid UserId, decimal TotalPrice, DateTimeOffset CheckIn, DateTimeOffset CheckOut)
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

public record BookingCancelledEvent(Guid BookingId, Guid PropertyId, string Reason)
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
