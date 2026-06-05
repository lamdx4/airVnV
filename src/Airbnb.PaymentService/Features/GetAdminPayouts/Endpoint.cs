using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GetAdminPayouts;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<PagedResponse<AdminPayoutResponse>>>
{
    public override void Configure()
    {
        Get("/api/payouts/admin");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: list all payouts with pagination and filters";
            s.Description = "Returns a paged list of all payouts for admin financial management.";
            s.Responses[200] = "Paginated list of payouts.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<PagedResponse<AdminPayoutResponse>>.SuccessResult(result), cancellation: ct);
    }
}
