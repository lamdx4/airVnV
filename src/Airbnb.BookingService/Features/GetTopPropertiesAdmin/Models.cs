using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetTopPropertiesAdmin;

public record Request(
    [property: BindFrom("from")] DateOnly From,
    [property: BindFrom("to")] DateOnly To,
    [property: BindFrom("limit")] int Limit = 10
) : Mediator.IQuery<ApiResponse<List<TopPropertyBasic>>>;

public record TopPropertyBasic(
    Guid PropertyId,
    decimal Revenue,
    int Bookings,
    decimal OccupancyRate
);
