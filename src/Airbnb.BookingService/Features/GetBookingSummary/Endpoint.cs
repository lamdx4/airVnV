using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetBookingSummary;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<BookingSummaryResponse>>
{
    public override void Configure()
    {
        Get("/api/bookings/admin/summary");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: get booking summary for a date range";
            s.Description = "Returns aggregated booking counts and revenue for Confirmed bookings within [from, to].";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
