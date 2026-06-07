using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetPropertiesByIds;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<List<PropertyBasicInfo>>>
{
    public override void Configure()
    {
        Get("/api/properties/bulk");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get rich property data for a list of property ids";
            s.Description = "Used to hydrate Search results with full images, prices, and ratings.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
