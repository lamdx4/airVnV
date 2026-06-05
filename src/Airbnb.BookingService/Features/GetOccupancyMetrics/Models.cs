using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetOccupancyMetrics;

public record Request(
    [property: BindFrom("from")] DateOnly From,
    [property: BindFrom("to")] DateOnly To
) : Mediator.IQuery<ApiResponse<OccupancyMetricsResponse>>;

public record OccupancyMetricsResponse(
    long BookedNights,
    int RangeDays
);
