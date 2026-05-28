using Airbnb.ChatService.Domain;
using Airbnb.ChatService.Infrastructure;
using Airbnb.ChatService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.ChatService.Features.Conversations.Create;

public sealed class Handler(AppDbContext db, PropertyServiceClient propertyClient) : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        // 1. Chống duplicate (Solution 2 - App-level check)
        var existingConversation = await db.Conversations
            .AsNoTracking()
            .Where(c => c.PropertyId == req.PropertyId && c.ReservationId == req.ReservationId)
            .Where(c => c.Participants.Any(p => p.UserId == req.GuestId && p.Role == ParticipantRole.Guest))
            .Select(c => c.Id)
            .FirstOrDefaultAsync(ct);

        if (existingConversation != Guid.Empty)
        {
            return new Response(existingConversation);
        }

        // 2. Lấy thông tin PropertyTitle và HostId
        var propertyInfo = await propertyClient.GetPropertyBasicInfoAsync(req.PropertyId, ct)
            ?? throw new NotFoundException("Property not found.");

        if (req.GuestId == propertyInfo.HostId)
        {
            throw new BusinessException("Host cannot start a conversation as a guest for their own property.", "CHAT_SELF_CONVERSATION");
        }

        // 3. Tạo Conversation
        var conversation = new Conversation
        {
            PropertyId = req.PropertyId,
            PropertyTitle = propertyInfo.Title, // Snapshot
            ReservationId = req.ReservationId,
            CreatedAt = DateTimeOffset.UtcNow,
            LastMessageAt = DateTimeOffset.UtcNow
        };

        db.Conversations.Add(conversation);

        // Ensure Guest ChatUser exists
        var guestUser = await db.ChatUsers.FindAsync([req.GuestId], ct);
        if (guestUser == null)
        {
            db.ChatUsers.Add(new ChatUser
            {
                UserId = req.GuestId,
                DisplayName = req.GuestName,
                AvatarUrl = req.GuestAvatarUrl
            });
        }

        // Ensure Host ChatUser exists
        var hostUser = await db.ChatUsers.FindAsync([propertyInfo.HostId], ct);
        if (hostUser == null)
        {
            db.ChatUsers.Add(new ChatUser
            {
                UserId = propertyInfo.HostId,
                DisplayName = "Host",
                AvatarUrl = null
            });
        }

        // 4. Add Guest Participant
        db.ConversationParticipants.Add(new ConversationParticipant
        {
            ConversationId = conversation.Id,
            UserId = req.GuestId,
            Role = ParticipantRole.Guest
        });

        // 5. Add Host Participant
        // (Trong thực tế ta sẽ cần gọi UserService để lấy tên Host, nhưng để tránh delay/HTTP chéo,
        // ta có thể lưu tạm 'Host' và chờ UserProfileUpdatedEvent đồng bộ, hoặc truyền từ request)
        db.ConversationParticipants.Add(new ConversationParticipant
        {
            ConversationId = conversation.Id,
            UserId = propertyInfo.HostId,
            Role = ParticipantRole.Host
        });

        await db.SaveChangesAsync(ct);

        return new Response(conversation.Id);
    }
}
