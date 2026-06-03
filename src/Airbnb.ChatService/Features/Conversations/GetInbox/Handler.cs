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
        // Sử dụng Select trung gian để cache CurrentParticipant, giúp EF Core không phải lặp lại subquery
        var rawConversations = await query
            .OrderByDescending(c => c.LastMessageAt)
            .Take(req.Limit + 1)
            .Select(c => new
            {
                Conversation = c,
                CurrentParticipant = c.Participants.FirstOrDefault(p => p.UserId == req.UserId),
                OtherParticipant = c.Participants
                    .Where(p => p.UserId != req.UserId)
                    .Select(p => new 
                    { 
                        p.UserId, 
                        DisplayName = p.User.DisplayName, 
                        AvatarUrl = p.User.AvatarUrl, 
                        p.LastReadMessageId 
                    })
                    .FirstOrDefault(),
                LatestMessage = c.Messages.OrderByDescending(m => m.CreatedAt)
                                          .Select(m => new { m.Id, m.Content, m.MessageType })
                                          .FirstOrDefault()
            })
            .Select(x => new
            {
                x.Conversation.Id,
                x.Conversation.PropertyId,
                x.Conversation.PropertyTitle,
                x.Conversation.LastMessageAt,
                x.OtherParticipant,
                UnreadCount = x.Conversation.Messages.Count(m => 
                    x.CurrentParticipant == null ||
                    x.CurrentParticipant.LastReadMessageId == null ||
                    m.Id.CompareTo(x.CurrentParticipant.LastReadMessageId.Value) > 0
                ),
                LatestMessageContent = x.LatestMessage != null ? x.LatestMessage.Content : null,
                LatestMessageId = x.LatestMessage != null ? (Guid?)x.LatestMessage.Id : null,
                LatestMessageType = x.LatestMessage != null ? x.LatestMessage.MessageType.ToString() : null
            })
            .ToListAsync(ct);

        var hasMore = rawConversations.Count > req.Limit;
        var results = rawConversations.Take(req.Limit).ToList();

        var items = results.Select(r => new InboxItem(
            r.Id,
            r.PropertyId,
            r.PropertyTitle,
            r.OtherParticipant?.DisplayName ?? "Unknown",
            r.OtherParticipant?.AvatarUrl,
            r.OtherParticipant?.UserId,
            r.UnreadCount,
            r.LastMessageAt,
            r.OtherParticipant?.LastReadMessageId,
            r.LatestMessageContent,
            r.LatestMessageId,
            r.LatestMessageType
        )).ToList();

        DateTimeOffset? nextCursor = hasMore ? items.Last().LastMessageAt : null;

        return new Response(items, nextCursor);
    }
}
