using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Airbnb.ChatService.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;

namespace Airbnb.ChatService.Features.Hubs;

public class ChatHub(AppDbContext db, IDistributedCache cache) : Hub
{
    private const string UserIdKey = "UserId";

    public override async Task OnConnectedAsync()
    {
        // Lấy UserId từ Header (do Gateway chuyển từ token (header/query) thành header X-User-Id)
        var httpContext = Context.GetHttpContext();
        var userIdStr = httpContext?.Request.Headers["X-User-Id"].ToString();

        if (!string.IsNullOrEmpty(userIdStr) && Guid.TryParse(userIdStr, out var userId))
        {
            // Lưu trữ UserId vào Context.Items để sử dụng lại trong suốt vòng đời của kết nối (Connection Lifecycle)
            Context.Items[UserIdKey] = userId;
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");

            // Set presence cache with 90s TTL
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(90)
            };
            await cache.SetStringAsync($"presence:user:{userId}", "online", options);

            await NotifyStatusChanged(userId, "online");
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

    public async Task SendTypingStatus(Guid conversationId, bool isTyping)
    {
        if (Context.Items.TryGetValue(UserIdKey, out var userIdObj) && userIdObj is Guid userId)
        {
            await Clients.OthersInGroup($"conv_{conversationId}").SendAsync("UserTyping", new 
            { 
                ConversationId = conversationId, 
                UserId = userId, 
                IsTyping = isTyping 
            });
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.Items.TryGetValue(UserIdKey, out var userIdObj) && userIdObj is Guid userId)
        {
            // Xóa connection khỏi user group (SignalR cũng tự động clear, nhưng thêm vào để tường minh hoặc xử lý thêm logic offline sau này)
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");

            // Xóa presence cache
            await cache.RemoveAsync($"presence:user:{userId}");

            await NotifyStatusChanged(userId, "offline");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task Heartbeat()
    {
        if (Context.Items.TryGetValue(UserIdKey, out var userIdObj) && userIdObj is Guid userId)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(90)
            };
            await cache.SetStringAsync($"presence:user:{userId}", "online", options);
        }
    }

    private async Task NotifyStatusChanged(Guid userId, string status)
    {
        var relatedUserIds = await db.ConversationParticipants
            .Where(p => db.ConversationParticipants
                .Where(cp => cp.UserId == userId)
                .Select(cp => cp.ConversationId)
                .Contains(p.ConversationId) && p.UserId != userId)
            .Select(p => p.UserId)
            .Distinct()
            .ToListAsync();

        if (relatedUserIds.Count != 0)
        {
            var groupNames = relatedUserIds.Select(id => $"user_{id}").ToList();
            await Clients.Groups(groupNames).SendAsync("UserStatusChanged", userId, status);
        }
    }
}
