using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.BookingService.Features.GetAdminStats;

public class Endpoint(IMediator mediator) : FastEndpoints.EndpointWithoutRequest<ApiResponse<BookingStatsResponse>>
{
    public override void Configure()
    {
        Get("/api/bookings/admin/stats");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: get booking statistics";
            s.Description = "Returns aggregate counts for bookings and revenue.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await mediator.Send(new Request(), ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
