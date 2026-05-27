using Airbnb.ChatService.Domain;
using Airbnb.ChatService.Features.Hubs;
using Airbnb.ChatService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.ChatService.Features.Conversations.SendMessage;

public sealed class Handler(AppDbContext db, IHubContext<ChatHub> hubContext) : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        // 1. Verify participant
        var conversation = await db.Conversations
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == req.ConversationId, ct);

        if (conversation == null)
            throw new NotFoundException("Conversation not found.");

        var senderParticipant = conversation.Participants.FirstOrDefault(p => p.UserId == req.SenderId);
        if (senderParticipant == null)
            throw new BusinessException("You are not a participant in this conversation.", "CHAT_ACCESS_DENIED");

        // 2. Tạo Message (dùng UUIDv7 nên tự sort theo time)
        var message = new Message
        {
            ConversationId = req.ConversationId,
            SenderId = req.SenderId,
            MessageType = MessageType.Text,
            Content = req.Content
        };

        db.Messages.Add(message);

        // 3. Update Conversation & Sender Read State
        conversation.LastMessageAt = message.CreatedAt;
        senderParticipant.LastReadMessageId = message.Id;

        await db.SaveChangesAsync(ct);

        // 4. Push SignalR tới những người trong Group conversation
        var messagePayload = new 
        {
            message.Id,
            message.ConversationId,
            message.SenderId,
            message.Content,
            message.CreatedAt
        };

        await hubContext.Clients.Group($"conv_{req.ConversationId}").SendAsync("ReceiveMessage", messagePayload, ct);

        // 5. Push SignalR tới user id group của tất cả những người tham gia
        var allUserGroups = conversation.Participants
            .Select(p => $"user_{p.UserId}")
            .ToList();

        if (allUserGroups.Count > 0)
        {
            await hubContext.Clients.Groups(allUserGroups).SendAsync("NewMessage", messagePayload, ct);
        }

        return new Response(message.Id, message.CreatedAt);
    }
}
