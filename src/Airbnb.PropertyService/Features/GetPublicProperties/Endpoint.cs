using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using Mediator;
using Airbnb.PropertyService.Features.GetAdminProperties;

namespace Airbnb.PropertyService.Features.GetPublicProperties;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<PagedResponse<Response>>>
{
    public override void Configure()
    {
        Get("/api/properties/public");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<PagedResponse<Response>>.SuccessResult(result), cancellation: ct);
    }
}
