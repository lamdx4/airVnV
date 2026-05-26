using Airbnb.PaymentService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;

namespace Airbnb.PaymentService.Features.AdminFinance;

public class PayoutsEndpoint(PaymentDbContext dbContext) : FastEndpoints.Endpoint<PayoutRequest, ApiResponse<PayoutResponse>>
{
    public override void Configure()
    {
        Get("/api/admin/finance/payouts");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(PayoutRequest req, CancellationToken ct)
    {
        var handler = new Handler(dbContext);
        var result = await handler.Handle(req, ct);
        await Send.ResponseAsync(ApiResponse<PayoutResponse>.SuccessResult(result), cancellation: ct);
    }
}
