using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetTopPropertiesAdmin;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<List<TopPropertyBasic>>>
{
    public override void Configure()
    {
        Get("/api/bookings/admin/top-properties");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: get top properties by revenue in a date range";
            s.Description = "Returns top N properties with their revenue, booking count, and occupancy rate. Title is joined by the caller.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
