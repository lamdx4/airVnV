using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetPublishedCount;

public class Endpoint(IMediator mediator)
    : FastEndpoints.EndpointWithoutRequest<ApiResponse<PublishedCountResponse>>
{
    public override void Configure()
    {
        Get("/api/properties/admin/published-count");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: count of Published properties";
            s.Description = "Used by UserService Reports to compute occupancy rate denominator.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await mediator.Send(new Request(), ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
