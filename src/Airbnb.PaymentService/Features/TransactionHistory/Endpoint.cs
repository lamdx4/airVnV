using Airbnb.PaymentService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;

namespace Airbnb.PaymentService.Features.TransactionHistory;

public class Endpoint(PaymentDbContext dbContext) : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/finance/transactions");
        Policies("AdminOnly");
        Summary(s =>
        {
            s.Summary = "Get transaction history with filters";
            s.Responses[200] = "List of transactions with summary";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var handler = new Handler(dbContext);
        var result = await handler.Handle(req, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
