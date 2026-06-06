using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetAdminBookings;

public record Request : Mediator.IQuery<ApiResponse<AdminBookingListResponse>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Search { get; init; }
    public string? Status { get; init; }
    public string? SortBy { get; init; }
    public string? SortOrder { get; init; }
}

public record AdminBookingResponse(
    Guid Id,
    Guid PropertyId,
    Guid HostId,
    Guid GuestId,
    string CountryCode,
    string BookingMode,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int GuestCount,
    int NightCount,
    decimal TotalPrice,
    string CurrencyCode,
    string Status,
    DateTimeOffset CreatedAt
);

public record AdminBookingListResponse(
    List<AdminBookingResponse> Items,
    int TotalItems,
    int Page,
    int PageSize,
    int TotalPages
);
