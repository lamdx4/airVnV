using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetOccupancyMetrics;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<OccupancyMetricsResponse>>
{
    public override void Configure()
    {
        Get("/api/bookings/admin/occupancy");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: get occupancy metrics (booked nights + range days)";
            s.Description = "Returns total booked nights for Confirmed bookings overlapping the range, and the range size in days. Caller combines with published-property count to compute occupancy rate.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
