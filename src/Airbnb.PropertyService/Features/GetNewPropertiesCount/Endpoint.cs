using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.GetNewPropertiesCount;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<NewPropertiesCountResponse>>
{
    public override void Configure()
    {
        Get("/api/properties/admin/new-count");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: count of new properties in a date range";
            s.Description = "Used by UserService Reports to compute new properties count in the date range.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
