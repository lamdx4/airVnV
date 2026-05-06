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
        // Giả sử Auth
        // AllowAnonymous(); 
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var senderId = string.IsNullOrEmpty(userIdClaim) ? req.SenderId : Guid.Parse(userIdClaim);
        
        var requestWithUser = req with { SenderId = senderId };

        var result = await mediator.Send(requestWithUser, ct);
        await SendAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
