using Airbnb.ChatService.Domain;
using Airbnb.ChatService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.ChatService.Features.Conversations.GetAttachments;

public sealed class Handler(AppDbContext db) : IQueryHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        // Kiểm tra quyền (phải là thành viên của conversation)
        var isParticipant = await db.ConversationParticipants
            .AnyAsync(p => p.ConversationId == req.ConversationId && p.UserId == req.UserId, ct);

        if (!isParticipant)
        {
            throw new BusinessException("You are not a participant in this conversation.", "CHAT_FORBIDDEN");
        }

        if (!Enum.TryParse<MessageType>(req.Type, true, out var messageType) || 
            (messageType != MessageType.Image && messageType != MessageType.File))
        {
            throw new BusinessException("Invalid attachment type. Must be Image or File.", "INVALID_ATTACHMENT_TYPE");
        }

        var query = db.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == req.ConversationId && m.MessageType == messageType);

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

        var items = results.Select(m => new AttachmentItem(
            m.Id,
            m.SenderId,
            m.Content,
            m.MessageType.ToString(),
            m.CreatedAt
        )).ToList();

        // Cursor để fetch trang tiếp theo là thời điểm của tin nhắn cũ nhất trong cục vừa lấy (kết quả đang sort desc, nên last là cũ nhất)
        DateTimeOffset? nextCursor = (hasMore && items.Count > 0) ? items.Last().CreatedAt : null;

        return new Response(items, nextCursor);
    }
}
