using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using Mediator;

namespace Airbnb.PropertyService.Features.CreateTax;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/admin/taxes");
        AllowAnonymous(); // Anonymous for Phase 2, later protected by Admin Auth
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
