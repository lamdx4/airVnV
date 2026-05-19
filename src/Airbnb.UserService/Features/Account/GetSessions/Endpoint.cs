using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Account.GetSessions;

public class Endpoint(IMediator mediator) : EndpointWithoutRequest<ApiResponse<List<SessionResponse>>>
{
    public override void Configure()
    {
        Get("/api/account/sessions");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await mediator.Send(new Request(), ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
