using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Airbnb.ChatService.Features.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // Khi user connect, join họ vào một nhóm mang tên UserId của họ để dễ push notification 1-1
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
        
        await base.OnConnectedAsync();
    }

    public async Task JoinConversation(Guid conversationId)
    {
        // TODO: Cần check xem user có nằm trong Conversation này không trước khi cho join
        // (Sẽ gọi Mediator Query ở đây hoặc làm Guard)
        var userId = Context.UserIdentifier;
        
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conv_{conversationId}");
    }

    public async Task LeaveConversation(Guid conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conv_{conversationId}");
    }
}
