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
        AllowAnonymous(); 
        Summary(s =>
        {
            s.Summary = "Lưu trữ (Archive) cuộc trò chuyện";
            s.Description = @"
**Error Codes:**
- **`CHAT_ACCESS_DENIED`**: Bạn không tham gia cuộc trò chuyện này.
";
            s.Responses[200] = "Archive thành công.";
            s.Responses[400] = "Lỗi nghiệp vụ.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userIdStr = HttpContext.Request.Headers["X-User-Id"].ToString();
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            await Send.ResponseAsync(ApiResponse<Response>.FailureResult("UNAUTHORIZED", "User identification missing."), 401, ct);
            return;
        }
        
        var requestWithUser = req with { UserId = userId };

        var result = await mediator.Send(requestWithUser, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
