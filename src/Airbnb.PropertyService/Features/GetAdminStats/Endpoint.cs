using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetAdminStats;

public class Endpoint(IMediator mediator) : FastEndpoints.EndpointWithoutRequest<ApiResponse<PropertyStatsResponse>>
{
    public override void Configure()
    {
        Get("/api/properties/admin/stats");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: get property statistics";
            s.Description = "Returns aggregate counts for properties and reviews.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await mediator.Send(new Request(), ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
