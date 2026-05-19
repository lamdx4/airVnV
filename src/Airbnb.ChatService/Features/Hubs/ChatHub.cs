using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Airbnb.ChatService.Features.Hubs;

public class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // Lấy UserId từ Header (do Gateway gắn vào) thông qua HttpContext
        var httpContext = Context.GetHttpContext();
        var userId = httpContext?.Request.Headers["X-User-Id"].ToString();

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
        
        await base.OnConnectedAsync();
    }

    public async Task JoinConversation(Guid conversationId)
    {
        var httpContext = Context.GetHttpContext();
        var userId = httpContext?.Request.Headers["X-User-Id"].ToString();
        
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conv_{conversationId}");
    }

    public async Task LeaveConversation(Guid conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conv_{conversationId}");
    }
}
