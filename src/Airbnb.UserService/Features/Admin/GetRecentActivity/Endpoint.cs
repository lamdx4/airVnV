using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.GetRecentActivity;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<List<ActivityItem>>>
{
    public override void Configure()
    {
        Get("/recent-activity");
        Group<DashboardGroup>();
        Roles("Admin", "Moderator");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
