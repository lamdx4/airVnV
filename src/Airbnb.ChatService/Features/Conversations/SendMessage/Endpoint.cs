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
        Summary(s =>
        {
            s.Summary = "Gửi tin nhắn vào cuộc trò chuyện";
            s.Description = @"
**Error Codes:**
- **`CHAT_ACCESS_DENIED`**: Bạn không tham gia cuộc trò chuyện này.
- **`CHAT_INVALID_MESSAGE_TYPE`**: Loại tin nhắn không hợp lệ.
- **`CHAT_SYSTEM_MESSAGE_FORBIDDEN`**: Không thể gửi tin nhắn hệ thống.
- **`NOT_FOUND`**: Không tìm thấy Conversation.
";
            s.Responses[200] = "Thành công. Trả về tin nhắn đã gửi.";
            s.Responses[400] = "Lỗi nghiệp vụ.";
            s.Responses[404] = "Không tìm thấy dữ liệu liên quan.";
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
        
        var requestWithUser = req with { SenderId = userId };

        var result = await mediator.Send(requestWithUser, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
