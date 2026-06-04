using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetRecentActivity;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<List<ActivityItem>>>
{
    public override void Configure()
    {
        Get("/api/properties/admin/recent-activity");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: get recent property/review activity";
            s.Description = "Returns recent property listings and reviews for the admin activity feed.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
