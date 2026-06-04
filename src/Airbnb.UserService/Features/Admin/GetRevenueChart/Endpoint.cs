using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetRevenueChart;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<List<RevenueChartPoint>>>
{
    public override void Configure()
    {
        Get("/revenue-chart");
        Group<DashboardGroup>();
        Roles("Admin", "Moderator");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
