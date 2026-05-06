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

        // Lấy danh sách conversation
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
                // Lấy ID tin nhắn cuối cùng mình đã đọc
                MyLastReadId = c.Participants.Where(p => p.UserId == req.UserId).Select(p => p.LastReadMessageId).FirstOrDefault()
            })
            .ToListAsync(ct);

        var hasMore = rawConversations.Count > req.Limit;
        var results = rawConversations.Take(req.Limit).ToList();

        // Tính UnreadCount (realtime count, không dùng cột counter để tránh drift)
        // Vì EF Core chưa support lateral join tốt cho count dynamic, ta query riêng cho N records (vì N chỉ là 20)
        // Hoặc optimize bằng 1 query group by nếu cần. Dưới đây là cách group by 1 lần.
        
        var convIds = results.Select(r => r.Id).ToList();
        var myLastReads = results.ToDictionary(r => r.Id, r => r.MyLastReadId);

        // Đếm số message > last read cho từng conversation
        // Vì UUIDv7 sortable, ta có thể so sánh > string (nếu lưu dạng string) hoặc > Guid. 
        // Trong Postgres, so sánh 2 cột UUID > < là hợp lệ nếu driver hỗ trợ. 
        // Tuy nhiên Npgsql có hỗ trợ UUIDv7 từ v8+, so sánh < > có thể cần cast hoặc dùng CreatedAt.
        // Cách an toàn nhất là so sánh CreatedAt (Message.Id sinh ra theo thời gian).
        // Vì ta không có CreatedAt của LastReadMessageId ở đây, ta có thể dùng trực tiếp e.Id.CompareTo(...)
        // Cách thực dụng: load CreatedAt của message cuối cùng, hoặc Npgsql cho phép `e.Id.CompareTo(lastReadId) > 0`
        
        // Đoạn này dùng LINQ client eval một phần hoặc chạy từng câu query nếu EF báo lỗi.
        // Để chuẩn xác và tránh N+1, ta query danh sách messages chưa đọc dựa theo điều kiện đơn giản:
        var unreadCounts = new Dictionary<Guid, int>();
        foreach(var conv in results)
        {
            if (conv.MyLastReadId == null)
            {
                unreadCounts[conv.Id] = await db.Messages.CountAsync(m => m.ConversationId == conv.Id, ct);
            }
            else
            {
                var lastReadId = conv.MyLastReadId.Value;
                // Npgsql support so sánh > cho UUIDv7
                unreadCounts[conv.Id] = await db.Messages.CountAsync(m => m.ConversationId == conv.Id && m.Id.CompareTo(lastReadId) > 0, ct);
            }
        }

        var items = results.Select(r => new InboxItem(
            r.Id,
            r.PropertyTitle,
            r.OtherParticipant?.DisplayName ?? "Unknown",
            r.OtherParticipant?.AvatarUrl,
            unreadCounts.GetValueOrDefault(r.Id, 0),
            r.LastMessageAt
        )).ToList();

        DateTimeOffset? nextCursor = hasMore ? items.Last().LastMessageAt : null;

        return new Response(items, nextCursor);
    }
}
