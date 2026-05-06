using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using Mediator;
using System.Security.Claims;

namespace Airbnb.ChatService.Features.Conversations.Archive;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Patch("/api/conversations/{ConversationId}/archive");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userId = string.IsNullOrEmpty(userIdClaim) ? req.UserId : Guid.Parse(userIdClaim);
        
        var requestWithUser = req with { UserId = userId };

        var result = await mediator.Send(requestWithUser, ct);
        await SendAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
