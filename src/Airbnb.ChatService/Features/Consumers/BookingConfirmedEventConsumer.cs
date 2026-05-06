using Airbnb.ChatService.Domain;
using Airbnb.ChatService.Infrastructure;
using Airbnb.SharedKernel.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.ChatService.Features.Consumers;

public class BookingConfirmedEventConsumer(AppDbContext db) : IConsumer<BookingConfirmedEvent>
{
    public async Task Consume(ConsumeContext<BookingConfirmedEvent> context)
    {
        var message = context.Message;

        // Tìm conversation giữa Guest và Host của Property này
        var conversation = await db.Conversations
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => 
                c.PropertyId == message.PropertyId && 
                c.Participants.Any(p => p.UserId == message.UserId && p.Role == ParticipantRole.Guest), 
                context.CancellationToken);

        if (conversation == null)
        {
            // Tùy nghiệp vụ: tự động tạo Conversation mới nếu chưa có, hoặc bỏ qua.
            // Ở Airbnb, khi booking confirm, hệ thống thường gửi system message vào conversation tồn tại, hoặc tạo mới.
            // Để an toàn, giả sử ta bỏ qua nếu chưa có (vì guest chưa nhắn gì), hoặc log lại.
            return;
        }

        // Cập nhật ReservationId cho Conversation này
        conversation.ReservationId = message.BookingId;

        // Tạo system message
        var systemMsg = new Message
        {
            ConversationId = conversation.Id,
            SenderId = null, // System Message
            MessageType = MessageType.System,
            Content = $"Booking confirmed! Check-in: {message.CheckIn:d}. Total: ${message.TotalPrice}",
        };

        db.Messages.Add(systemMsg);
        
        conversation.LastMessageAt = systemMsg.CreatedAt;

        await db.SaveChangesAsync(context.CancellationToken);

        // TODO: Push SignalR update to Participants
    }
}
