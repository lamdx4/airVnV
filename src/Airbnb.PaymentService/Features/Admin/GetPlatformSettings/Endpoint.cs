using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetPlatformSettings;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/admin/settings/platform");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: get current platform fee settings";
            s.Description = "No error codes. Returns existing settings, or creates a default row on first call.";
            s.Responses[200] = "Platform settings retrieved";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        Response = ApiResponse<Response>.SuccessResult(result);
    }
}
