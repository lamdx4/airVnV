using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.ExecutePayout;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<FastEndpoints.EmptyRequest, ApiResponse<PayoutActionResponse>>
{
    public override void Configure()
    {
        Patch("/api/payouts/admin/{PayoutId}/execute");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: execute an approved payout";
            s.Description = "Transitions an Approved payout to Processing, then Completed. " +
                            "In production, this would initiate a bank transfer.";
            s.Responses[200] = "Payout executed.";
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
