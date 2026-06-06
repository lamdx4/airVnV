using Airbnb.ChatService.Domain;
using Airbnb.ChatService.Infrastructure;
using Airbnb.ChatService.Infrastructure.HttpClients;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.ChatService.Features.Conversations.Create;

public sealed class Handler(AppDbContext db, PropertyServiceClient propertyClient, UserServiceClient userClient) : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        // 1. Chống duplicate an toàn tuyệt đối
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

        // 3. Đảm bảo ChatUser tồn tại (dùng SQL Upsert để triệt tiêu Race Condition khi insert)
        var guestProfileTask = userClient.GetPublicProfileAsync(req.GuestId, ct);
        var hostProfileTask = userClient.GetPublicProfileAsync(propertyInfo.HostId, ct);
        await Task.WhenAll(guestProfileTask, hostProfileTask);

        var guestProfile = guestProfileTask.Result;
        var hostProfile = hostProfileTask.Result;

        var guestName = guestProfile?.FullName ?? "Guest";
        var guestAvatar = guestProfile?.AvatarUrl;
        var hostName = hostProfile?.FullName ?? "Host";
        var hostAvatar = hostProfile?.AvatarUrl;

        // Chạy Raw SQL để "INSERT IF NOT EXISTS" an toàn tuyệt đối trong Postgres
        await db.Database.ExecuteSqlAsync($@"
            INSERT INTO ""ChatUsers"" (""UserId"", ""DisplayName"", ""AvatarUrl"")
            VALUES ({req.GuestId}, {guestName}, {guestAvatar})
            ON CONFLICT (""UserId"") DO NOTHING;
        ", ct);

        await db.Database.ExecuteSqlAsync($@"
            INSERT INTO ""ChatUsers"" (""UserId"", ""DisplayName"", ""AvatarUrl"")
            VALUES ({propertyInfo.HostId}, {hostName}, {hostAvatar})
            ON CONFLICT (""UserId"") DO NOTHING;
        ", ct);

        // 4. Tạo Conversation
        var conversation = new Conversation
        {
            PropertyId = req.PropertyId,
            PropertyTitle = propertyInfo.Title, // Snapshot
            ReservationId = req.ReservationId,
            CreatedAt = DateTimeOffset.UtcNow,
            LastMessageAt = DateTimeOffset.UtcNow
        };

        db.Conversations.Add(conversation);

        // 5. Thêm Participants
        db.ConversationParticipants.Add(new ConversationParticipant
        {
            ConversationId = conversation.Id,
            UserId = req.GuestId,
            Role = ParticipantRole.Guest
        });

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
