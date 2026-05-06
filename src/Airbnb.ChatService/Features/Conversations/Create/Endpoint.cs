using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using Mediator;
using System.Security.Claims;

namespace Airbnb.ChatService.Features.Conversations.Create;

public class Endpoint(IMediator mediator) : FastEndpoints.Endpoint<Request, ApiResponse<Response>>
{
    public override void Configure()
    {
        Post("/api/conversations");
        // Giả sử lấy UserId từ JWT
        // AllowAnonymous(); 
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // Ghi đè GuestId từ Claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var guestId = string.IsNullOrEmpty(userIdClaim) ? req.GuestId : Guid.Parse(userIdClaim);
        
        var requestWithUser = req with { GuestId = guestId };

        var result = await mediator.Send(requestWithUser, ct);
        await SendAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
