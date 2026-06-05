using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetRevenueBreakdown;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<List<RevenueBreakdownPoint>>>
{
    public override void Configure()
    {
        Get("/api/bookings/admin/revenue-breakdown");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: get revenue breakdown by period";
            s.Description = "Returns revenue, bookings, and cancellations grouped by day/week/month for the date range.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
