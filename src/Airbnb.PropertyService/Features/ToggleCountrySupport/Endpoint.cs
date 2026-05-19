using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using Mediator;

namespace Airbnb.PropertyService.Features.ToggleCountrySupport;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Patch("/api/admin/countries/{CountryCode}/toggle");
        AllowAnonymous(); // Currently anonymous for Phase 2 API-only admin access
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
