using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetPropertiesByIds;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<List<PropertyBasicInfo>>>
{
    public override void Configure()
    {
        Get("/api/properties/admin/by-ids");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: get basic info (id, title) for a list of property ids";
            s.Description = "Used by UserService Reports to enrich TopProperties with titles.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
