using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetAdminPayoutDetail;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/payouts/{id}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: get payout detail with line items";
            s.Description = "**Possible error codes:**\n- `NOT_FOUND` — payout does not exist";
            s.Responses[200] = "Payout detail retrieved";
            s.Responses[404] = "Payout not found";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        Response = ApiResponse<Response>.SuccessResult(result);
    }
}
