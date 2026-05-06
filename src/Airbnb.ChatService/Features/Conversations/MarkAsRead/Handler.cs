using Airbnb.ChatService.Features.Hubs;
using Airbnb.ChatService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.ChatService.Features.Conversations.MarkAsRead;

public sealed class Handler(AppDbContext db, IHubContext<ChatHub> hubContext) : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var participant = await db.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == req.ConversationId && p.UserId == req.UserId, ct);

        if (participant == null)
        {
            throw new BusinessException("You do not have access to this conversation.", "CHAT_ACCESS_DENIED");
        }

        participant.LastReadMessageId = req.LastReadMessageId;

        await db.SaveChangesAsync(ct);

        // Push SignalR "MessageRead" để client bên kia biết (tạo UX seen)
        await hubContext.Clients.Group($"conv_{req.ConversationId}").SendAsync("MessageRead", new
        {
            req.ConversationId,
            ReaderId = req.UserId,
            req.LastReadMessageId
        }, ct);

        return new Response(true);
    }
}
