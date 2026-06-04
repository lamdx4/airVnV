using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetAdminStats;

public record Request : Mediator.IQuery<ApiResponse<BookingStatsResponse>>;

public record BookingStatsResponse(
    int TotalBookings,
    int ActiveBookings,
    decimal TotalRevenue
);
