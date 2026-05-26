using Airbnb.ChatService.Domain;
using Airbnb.ChatService.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.ChatService.Features.Conversations.GetInbox;

public sealed class Handler(AppDbContext db) : IQueryHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var query = db.Conversations
            .AsNoTracking()
            .Where(c => c.Participants.Any(p => p.UserId == req.UserId && !p.IsArchived));

        if (req.Before.HasValue)
        {
            query = query.Where(c => c.LastMessageAt < req.Before.Value);
        }

        // Lấy danh sách conversation kèm theo đếm số tin nhắn chưa đọc trong cùng 1 câu query
        var rawConversations = await query
            .OrderByDescending(c => c.LastMessageAt)
            .Take(req.Limit + 1)
            .Select(c => new
            {
                c.Id,
                c.PropertyTitle,
                c.LastMessageAt,
                // Lấy thông tin Participant khác
                OtherParticipant = c.Participants.FirstOrDefault(p => p.UserId != req.UserId),
                // Đếm tin nhắn chưa đọc trực tiếp bằng cách so sánh Id tin nhắn (UUIDv7) với LastReadMessageId của user hiện tại
                UnreadCount = c.Messages.Count(m => 
                    c.Participants.Where(p => p.UserId == req.UserId).Select(p => p.LastReadMessageId).FirstOrDefault() == null ||
                    m.Id.CompareTo(c.Participants.Where(p => p.UserId == req.UserId).Select(p => p.LastReadMessageId).FirstOrDefault()!.Value) > 0
                )
            })
            .ToListAsync(ct);

        var hasMore = rawConversations.Count > req.Limit;
        var results = rawConversations.Take(req.Limit).ToList();

        var items = results.Select(r => new InboxItem(
            r.Id,
            r.PropertyTitle,
            r.OtherParticipant?.DisplayName ?? "Unknown",
            r.OtherParticipant?.AvatarUrl,
            r.UnreadCount,
            r.LastMessageAt,
            r.OtherParticipant?.LastReadMessageId
        )).ToList();

        DateTimeOffset? nextCursor = hasMore ? items.Last().LastMessageAt : null;

        return new Response(items, nextCursor);
    }
}
