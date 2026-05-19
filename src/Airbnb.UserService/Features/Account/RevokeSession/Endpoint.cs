using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Account.RevokeSession;

public class Endpoint(IMediator mediator) : Endpoint<Request, ApiResponse<bool>>
{
    public override void Configure()
    {
        Post("/api/account/sessions/revoke");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
