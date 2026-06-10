using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetHostBalances;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/host-balances");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: list host balances (platform escrow ledger)";
            s.Description = "**Possible error codes:**\n- `VALIDATION_ERROR` — invalid query parameters";
            s.Responses[200] = "Host balances retrieved";
            s.Responses[400] = "Validation error";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        Response = ApiResponse<Response>.SuccessResult(result);
    }
}
