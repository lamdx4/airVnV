using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GeneratePayouts;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<FastEndpoints.EmptyRequest, ApiResponse<GeneratePayoutsResponse>>
{
    public override void Configure()
    {
        Post("/api/payouts/admin/generate");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: generate pending payouts from completed bookings";
            s.Description = "Scans for successful payments not yet included in any payout, " +
                            "groups them by Host, and creates Pending payout records.";
            s.Responses[200] = "Payouts generated successfully.";
        });
    }

    public override async Task HandleAsync(FastEndpoints.EmptyRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new Command(), ct);
        await Send.ResponseAsync(ApiResponse<GeneratePayoutsResponse>.SuccessResult(result), cancellation: ct);
    }
}
