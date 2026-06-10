using FastEndpoints;
using Mediator;
using System.Security.Claims;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.ApprovePayout;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/admin/payouts/{id}/approve");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: approve a pending payout. Releases pending → available, then deducts payout amount from available.";
            s.Description = "**Possible error codes:**\n- `NOT_FOUND` — payout does not exist\n- `PAYOUT_NO_LEDGER` — host balance ledger missing (PaymentSucceeded consumer did not run)";
            s.Responses[200] = "Payout approved and ledger updated";
            s.Responses[400] = "Business rule violation";
            s.Responses[404] = "Payout not found";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var adminId);
        var performedBy = adminId == Guid.Empty ? Guid.CreateVersion7() : adminId;

        var result = await mediator.Send(req with { Id = id, PerformedBy = performedBy }, ct);
        Response = ApiResponse<Response>.SuccessResult(result, "Payout approved and ledger updated");
    }
}
