using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GetAdminPayoutDetail;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<AdminPayoutDetailResponse>>
{
    public override void Configure()
    {
        Get("/api/payouts/admin/{PayoutId}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: get payout detail with line items";
            s.Description = "Returns full payout details including line-item breakdown for each included booking.";
            s.Responses[200] = "Payout detail with items.";
            s.Responses[404] = "Payout not found.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<AdminPayoutDetailResponse>.SuccessResult(result), cancellation: ct);
    }
}
