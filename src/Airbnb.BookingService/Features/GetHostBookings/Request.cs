using Mediator;

namespace Airbnb.BookingService.Features.GetHostBookings;

public record BookingDto(
    Guid Id, 
    Guid PropertyId, 
    Guid HostId,
    Guid GuestId,
    DateOnly CheckIn, 
    DateOnly CheckOut, 
    int GuestCount,
    int NightCount,
    decimal TotalPrice, 
    string CurrencyCode,
    string Status,
    string BookingMode);

public record Request : IQuery<List<BookingDto>>
{
    [FastEndpoints.FromHeader("X-User-Id")]
    public Guid HostId { get; init; }
}
