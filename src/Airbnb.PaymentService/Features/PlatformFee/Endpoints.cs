using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;

namespace Airbnb.PaymentService.Features.PlatformFee;

public class Endpoint : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/finance/platform-fee");
        Policies("AdminOnly");
        Summary(s =>
        {
            s.Summary = "Get platform fee configuration";
            s.Responses[200] = "Current platform fee settings";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var handler = new Handler();
        var result = handler.Handle(req, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}

public class UpdateEndpoint : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Put("/api/admin/finance/platform-fee");
        Policies("AdminOnly");
        Summary(s =>
        {
            s.Summary = "Update platform fee configuration";
            s.Responses[200] = "Updated platform fee settings";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var handler = new Handler();
        var result = handler.Handle(req, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
