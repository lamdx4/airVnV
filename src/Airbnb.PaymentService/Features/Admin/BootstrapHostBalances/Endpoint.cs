using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.BootstrapHostBalances;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/host-balances/bootstrap");
        Post("/api/admin/host-balances/bootstrap");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin (dev tool): wipe + rebuild payouts + escrow ledger from existing payments";
            s.Description = "No error codes. Dev/demo tool that re-derives ledger data from Success payments.";
            s.Responses[200] = "Bootstrap completed (or no-op when no payments)";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        Response = ApiResponse<Response>.SuccessResult(result, result.Message);
    }
}
