using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetBookingSummary;

public record Request(
    [property: BindFrom("from")] DateOnly From,
    [property: BindFrom("to")] DateOnly To
) : Mediator.IQuery<ApiResponse<BookingSummaryResponse>>;

public record BookingSummaryResponse(
    int TotalBookings,
    decimal TotalRevenue,
    decimal AverageBookingValue
);
