using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetDashboardStats;

public class Endpoint(IMediator mediator) : FastEndpoints.EndpointWithoutRequest<ApiResponse<DashboardStatsResponse>>
{
    public override void Configure()
    {
        Get("/stats");
        Group<DashboardGroup>();
        Roles("Admin", "Moderator");
        Summary(s =>
        {
            s.Summary = "Admin: get dashboard statistics";
            s.Description = "Returns aggregated counts from all services for the admin dashboard.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await mediator.Send(new Request(), ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
