using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using Mediator;

namespace Airbnb.PropertyService.Features.GetProperty;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<PropertyDto>>
{
    public override void Configure()
    {
        Get("/api/properties/{PropertyId:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<PropertyDto>.SuccessResult(result), cancellation: ct);
    }
}
