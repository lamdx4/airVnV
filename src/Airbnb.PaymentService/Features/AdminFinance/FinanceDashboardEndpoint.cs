using Airbnb.PaymentService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;

namespace Airbnb.PaymentService.Features.AdminFinance;

public class FinanceDashboardEndpoint(PaymentDbContext dbContext) : FastEndpoints.Endpoint<DashboardRequest, ApiResponse<DashboardFinanceResponse>>
{
    public override void Configure()
    {
        Get("/api/admin/finance/dashboard");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(DashboardRequest req, CancellationToken ct)
    {
        var handler = new Handler(dbContext);
        var result = await handler.GetDashboardStats(req, ct);
        await Send.ResponseAsync(ApiResponse<DashboardFinanceResponse>.SuccessResult(result), cancellation: ct);
    }
}
