using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.MarkPayoutCompleted;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/admin/payouts/{id}/mark-completed");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: mark a payout as completed (bank transfer confirmed)";
            s.Description = "**Possible error codes:**\n- `NOT_FOUND` — payout does not exist\n- Business errors from `Payout.MarkCompleted()` (e.g. invalid state transition)";
            s.Responses[200] = "Payout marked as completed";
            s.Responses[400] = "Business rule violation";
            s.Responses[404] = "Payout not found";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await mediator.Send(req with { Id = id }, ct);
        Response = ApiResponse<Response>.SuccessResult(result, "Payout marked as completed");
    }
}
