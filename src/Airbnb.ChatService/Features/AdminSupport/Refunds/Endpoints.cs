using Airbnb.ChatService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;

namespace Airbnb.ChatService.Features.AdminSupport.Refunds;

public class GetRefundsEndpoint(AppDbContext dbContext) : FastEndpoints.Endpoint<GetRefundsRequest, ApiResponse<RefundListResponse>>
{
    public override void Configure()
    {
        Get("/api/admin/support/refunds");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(GetRefundsRequest req, CancellationToken ct)
    {
        var handler = new RefundHandler(dbContext);
        var result = await handler.GetRefunds(req, ct);
        await Send.ResponseAsync(ApiResponse<RefundListResponse>.SuccessResult(result), cancellation: ct);
    }
}

public class ProcessRefundEndpoint(AppDbContext dbContext) : FastEndpoints.Endpoint<ProcessRefundRequest, ApiResponse<RefundResponse>>
{
    public override void Configure()
    {
        Post("/api/admin/support/refunds/process");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(ProcessRefundRequest req, CancellationToken ct)
    {
        var handler = new RefundHandler(dbContext);
        // TODO: Get adminId and adminName from JWT claims
        var result = await handler.ProcessRefund(req, Guid.Empty, "admin", ct);
        await Send.ResponseAsync(ApiResponse<RefundResponse>.SuccessResult(result), cancellation: ct);
    }
}

public class CreateRefundEndpoint(AppDbContext dbContext) : FastEndpoints.Endpoint<CreateRefundRequest, ApiResponse<RefundResponse>>
{
    public override void Configure()
    {
        Post("/api/admin/support/refunds");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(CreateRefundRequest req, CancellationToken ct)
    {
        var handler = new RefundHandler(dbContext);
        var result = await handler.CreateRefund(req, ct);
        await Send.ResponseAsync(ApiResponse<RefundResponse>.SuccessResult(result), cancellation: ct);
    }
}
