using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using Mediator;
using System.Security.Claims;

namespace Airbnb.ChatService.Features.Conversations.SendMessage;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/conversations/{ConversationId}/messages");
        AllowAnonymous(); 
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userIdStr = HttpContext.Request.Headers["X-User-Id"].ToString();
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            await Send.ResponseAsync(ApiResponse<Response>.FailureResult("UNAUTHORIZED", "User identification missing."), 401, ct);
            return;
        }
        
        var requestWithUser = req with { SenderId = userId };

        var result = await mediator.Send(requestWithUser, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
