using Airbnb.ChatService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.ChatService.Features.Conversations.Archive;

public sealed class Handler(AppDbContext db) : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var participant = await db.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == req.ConversationId && p.UserId == req.UserId, ct);

        if (participant == null)
        {
            throw new BusinessException("You do not have access to this conversation.", "CHAT_ACCESS_DENIED");
        }

        participant.IsArchived = true;

        await db.SaveChangesAsync(ct);

        return new Response(true);
    }
}
