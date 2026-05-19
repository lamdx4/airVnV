using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.UserService.Infrastructure;

namespace Airbnb.UserService.Features.GoogleAuth.Execute;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/users/google-auth");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var mediatorReq = req with { UserAgent = userAgent, IpAddress = ipAddress };
        
        var result = await mediator.Send(mediatorReq, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
