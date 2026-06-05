using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.GetAdminPlatformFeeCurrent;

public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<FastEndpoints.EmptyRequest, ApiResponse<PlatformFeeCurrentResponse>>
{
    public override void Configure()
    {
        Get("/api/platform-fee/admin/current");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: get current active platform fee configuration";
            s.Description = "Returns the currently active platform fee percentage.";
            s.Responses[200] = "Current platform fee configuration.";
            s.Responses[404] = "No active configuration found.";
        });
    }

    public override async Task HandleAsync(FastEndpoints.EmptyRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new Request(), ct);
        await Send.ResponseAsync(ApiResponse<PlatformFeeCurrentResponse>.SuccessResult(result), cancellation: ct);
    }
}
