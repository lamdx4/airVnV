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
            // Xóa connection khỏi user group (SignalR cũng tự động clear, nhưng thêm vào để tường minh)
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");

            // KHÔNG xóa presence cache hay báo "offline" ngay lập tức ở đây.
            // Tránh lỗi Multi-tab: Nếu user đóng 1 tab nhưng vẫn còn tab khác, họ không được phép bị offline.
            // Trạng thái Offline thực sự sẽ xảy ra một cách tự nhiên sau 90s khi Redis TTL hết hạn (vì không còn tab nào bơm Heartbeat nữa).
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

    // WebRTC Signaling Methods
    public async Task InitCall(Guid targetUserId, object offer, bool isVideoCall)
    {
        if (Context.Items.TryGetValue(UserIdKey, out var callerIdObj) && callerIdObj is Guid callerId)
        {
            await EnsureSharedConversation(callerId, targetUserId);

            await Clients.Group($"user_{targetUserId}").SendAsync("IncomingCall", new 
            { 
                CallerId = callerId, 
                Offer = offer,
                IsVideoCall = isVideoCall
            });
        }
    }

    public async Task AnswerCall(Guid targetUserId, object answer)
    {
        if (Context.Items.TryGetValue(UserIdKey, out var answererIdObj) && answererIdObj is Guid answererId)
        {
            await EnsureSharedConversation(answererId, targetUserId);
            await Clients.Group($"user_{targetUserId}").SendAsync("CallAnswered", new 
            { 
                AnswererId = answererId, 
                Answer = answer 
            });
        }
    }

    public async Task SendIceCandidate(Guid targetUserId, object candidate)
    {
        if (Context.Items.TryGetValue(UserIdKey, out var senderIdObj) && senderIdObj is Guid senderId)
        {
            await EnsureSharedConversation(senderId, targetUserId);
            await Clients.Group($"user_{targetUserId}").SendAsync("ReceiveIceCandidate", new 
            { 
                SenderId = senderId, 
                Candidate = candidate 
            });
        }
    }

    public async Task RejectCall(Guid targetUserId)
    {
        if (Context.Items.TryGetValue(UserIdKey, out var rejecterIdObj) && rejecterIdObj is Guid rejecterId)
        {
            await EnsureSharedConversation(rejecterId, targetUserId);
            await Clients.Group($"user_{targetUserId}").SendAsync("CallRejected", new 
            { 
                RejecterId = rejecterId 
            });
        }
    }

    public async Task EndCall(Guid targetUserId)
    {
        if (Context.Items.TryGetValue(UserIdKey, out var enderIdObj) && enderIdObj is Guid enderId)
        {
            await EnsureSharedConversation(enderId, targetUserId);
            await Clients.Group($"user_{targetUserId}").SendAsync("CallEnded", new 
            { 
                EnderId = enderId 
            });
        }
    }

    private async Task EnsureSharedConversation(Guid userId1, Guid userId2)
    {
        var hasSharedConversation = await db.Conversations
            .AnyAsync(c => c.Participants.Any(p => p.UserId == userId1) 
                        && c.Participants.Any(p => p.UserId == userId2));

        if (!hasSharedConversation)
        {
            throw new HubException("Forbidden: You must have an active conversation with this user.");
        }
    }
}
