using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetRevenueChart;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<List<RevenueChartPoint>>>
{
    public override void Configure()
    {
        Get("/api/bookings/admin/revenue-chart");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: get revenue chart data";
            s.Description = "Returns daily revenue and booking counts for the last N days.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
