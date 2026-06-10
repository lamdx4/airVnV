using FastEndpoints;
using Mediator;
using System.Security.Claims;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.UpdatePlatformSettings;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Put("/api/admin/settings/platform");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Admin: update platform fee settings";
            s.Description = "**Possible error codes:**\n- `VALIDATION_ERROR` — invalid body";
            s.Responses[200] = "Settings updated";
            s.Responses[400] = "Validation error";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var actor = User.FindFirstValue(ClaimTypes.Email)
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await mediator.Send(req with { Actor = actor }, ct);
        Response = ApiResponse<Response>.SuccessResult(result, "Settings updated");
    }
}
