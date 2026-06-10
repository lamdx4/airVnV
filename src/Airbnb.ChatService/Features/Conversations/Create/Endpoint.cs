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
        AllowAnonymous(); 
        Summary(s =>
        {
            s.Summary = "Tạo mới cuộc trò chuyện";
            s.Description = @"
**Error Codes:**
- **`CHAT_SELF_CONVERSATION`**: Host không thể tự tạo cuộc trò chuyện cho chính phòng của mình.
- **`NOT_FOUND`**: Không tìm thấy Property yêu cầu.
";
            s.Responses[200] = "Thành công. Trả về Id của cuộc trò chuyện.";
            s.Responses[400] = "Lỗi nghiệp vụ.";
            s.Responses[404] = "Không tìm thấy dữ liệu liên quan.";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            await Send.ResponseAsync(ApiResponse<Response>.FailureResult("UNAUTHORIZED", "User identification missing."), 401, ct);
            return;
        }
        
        var requestWithUser = req with { GuestId = userId };

        var result = await mediator.Send(requestWithUser, ct);
        await Send.ResponseAsync(ApiResponse<Response>.SuccessResult(result), cancellation: ct);
    }
}
