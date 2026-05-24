using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Airbnb.ChatService.Infrastructure;

namespace Airbnb.ChatService.Features.Hubs;

public class ChatHub(AppDbContext db) : Hub
{
    private const string UserIdKey = "UserId";

    public override async Task OnConnectedAsync()
    {
        // Lấy UserId từ Header (do Gateway gắn vào) thông qua HttpContext
        var httpContext = Context.GetHttpContext();
        var userIdStr = httpContext?.Request.Headers["X-User-Id"].ToString();

        if (string.IsNullOrEmpty(userIdStr))
        {
            // Fallback cho WebSockets chạy từ trình duyệt (do trình duyệt không hỗ trợ gửi custom headers qua WebSockets)
            userIdStr = httpContext?.Request.Query["userId"].ToString() 
                        ?? httpContext?.Request.Query["X-User-Id"].ToString();
        }

        if (!string.IsNullOrEmpty(userIdStr) && Guid.TryParse(userIdStr, out var userId))
        {
            // Lưu trữ UserId vào Context.Items để sử dụng lại trong suốt vòng đời của kết nối (Connection Lifecycle)
            Context.Items[UserIdKey] = userId;
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
        
        await base.OnConnectedAsync();
    }

    public async Task JoinConversation(Guid conversationId)
    {
        if (!Context.Items.TryGetValue(UserIdKey, out var userIdObj) || userIdObj is not Guid userId)
        {
            throw new HubException("Unauthorized: User identification missing.");
        }

        // Kiểm tra xem User này có thực sự tham gia vào phòng chat này không
        var isParticipant = await db.ConversationParticipants
            .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (!isParticipant)
        {
            throw new HubException("Forbidden: You do not have access to this conversation.");
        }
        
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conv_{conversationId}");
    }

    public async Task LeaveConversation(Guid conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conv_{conversationId}");
    }
}
