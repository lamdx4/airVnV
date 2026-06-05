using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GetAdminPlatformFeeHistory;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<Request, ApiResponse<PagedResponse<PlatformFeeHistoryItem>>>
{
    public override void Configure()
    {
        Get("/api/platform-fee/admin/history");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: get platform fee configuration history";
            s.Description = "Returns a paginated list of all platform fee configurations, most recent first.";
            s.Responses[200] = "Platform fee history.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<PagedResponse<PlatformFeeHistoryItem>>.SuccessResult(result), cancellation: ct);
    }
}
