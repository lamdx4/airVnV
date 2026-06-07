using FastEndpoints;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.ChatService.Features.GetUserStatus;

public class Endpoint(IMediator mediator)
    : Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Get("/api/conversations/users/{userId}/status");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
