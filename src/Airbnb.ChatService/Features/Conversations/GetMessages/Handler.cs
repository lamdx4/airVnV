using Airbnb.ChatService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.ChatService.Features.Conversations.GetMessages;

public sealed class Handler(AppDbContext db) : IQueryHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        // 1. Check access
        var isParticipant = await db.ConversationParticipants
            .AnyAsync(p => p.ConversationId == req.ConversationId && p.UserId == req.UserId, ct);

        if (!isParticipant)
        {
            throw new BusinessException("You do not have access to this conversation.", "CHAT_ACCESS_DENIED");
        }

        // 2. Query Messages
        var query = db.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == req.ConversationId);

        if (req.Before.HasValue)
        {
            query = query.Where(m => m.CreatedAt < req.Before.Value);
        }

        var rawMessages = await query
            .OrderByDescending(m => m.CreatedAt)
            .Take(req.Limit + 1)
            .ToListAsync(ct);

        var hasMore = rawMessages.Count > req.Limit;
        var results = rawMessages.Take(req.Limit).ToList();

        var items = results.Select(m => new MessageItem(
            m.Id,
            m.SenderId,
            m.Content,
            m.MessageType.ToString(),
            m.CreatedAt
        )).ToList();

        // Sort lại ASC để client hiển thị từ trên xuống dưới (cũ -> mới)
        items.Reverse();

        // Cursor để fetch trang tiếp theo là thời điểm của tin nhắn cũ nhất trong cục vừa lấy
        // Vì list đã bị reverse (cũ nhất ở index 0), nên:
        DateTimeOffset? nextCursor = hasMore ? items.First().CreatedAt : null;

        return new Response(items, nextCursor);
    }
}
