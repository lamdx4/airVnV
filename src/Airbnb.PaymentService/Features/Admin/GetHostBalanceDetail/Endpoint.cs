using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetHostBalanceDetail;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/host-balances/{id}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: host balance detail with ledger entries";
            s.Description = "**Possible error codes:**\n- `NOT_FOUND` — host balance does not exist";
            s.Responses[200] = "Host balance detail retrieved";
            s.Responses[404] = "Host balance not found";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        Response = ApiResponse<Response>.SuccessResult(result);
    }
}
