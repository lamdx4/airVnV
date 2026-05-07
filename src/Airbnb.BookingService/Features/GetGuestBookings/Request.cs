using Mediator;

namespace Airbnb.BookingService.Features.GetGuestBookings;

public record BookingDto(
    Guid Id, 
    Guid PropertyId, 
    Guid HostId,
    DateOnly CheckIn, 
    DateOnly CheckOut, 
    int GuestCount,
    int NightCount,
    decimal TotalPrice, 
    string CurrencyCode,
    string Status);

public record Request : IQuery<List<BookingDto>>
{
    [FastEndpoints.FromHeader("X-User-Id")]
    public Guid GuestId { get; init; }
}
