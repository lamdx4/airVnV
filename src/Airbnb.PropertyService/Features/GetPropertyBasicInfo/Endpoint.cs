using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using Mediator;

namespace Airbnb.PropertyService.Features.GetPropertyBasicInfo;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/properties/{PropertyId}/basic-info");
        AllowAnonymous(); // Tùy thuộc vào yêu cầu bảo mật, có thể đổi
        Summary(s =>
        {
            s.Summary = "Get basic information of a property (for internal microservice communication)";
            s.Responses[200] = "Basic property info returned successfully.";
            s.Responses[404] = "Property not found.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
