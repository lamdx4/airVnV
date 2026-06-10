using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PaymentService.Features.Admin.GetPlatformSettings;

public class Endpoint(IMediator mediator) : EndpointWithoutRequest<ApiResponse<Response>>
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

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await mediator.Send(new Request(), ct);
        Response = ApiResponse<Response>.SuccessResult(result);
    }
}
