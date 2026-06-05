using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.ApprovePayout;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<FastEndpoints.EmptyRequest, ApiResponse<PayoutActionResponse>>
{
    public override void Configure()
    {
        Patch("/api/payouts/admin/{PayoutId}/approve");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: approve a pending payout";
            s.Description = "Transitions a Pending payout to Approved status.";
            s.Responses[200] = "Payout approved.";
            s.Responses[400] = "Invalid status transition.";
            s.Responses[404] = "Payout not found.";
        });
    }

    public override async Task HandleAsync(FastEndpoints.EmptyRequest req, CancellationToken ct)
    {
        var payoutId = Route<Guid>("PayoutId");
        var result = await mediator.Send(new Command(payoutId), ct);
        await Send.ResponseAsync(ApiResponse<PayoutActionResponse>.SuccessResult(result), cancellation: ct);
    }
}
