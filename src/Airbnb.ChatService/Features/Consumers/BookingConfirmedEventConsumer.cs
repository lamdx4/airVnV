using Airbnb.ChatService.Domain;
using Airbnb.ChatService.Infrastructure;
using Airbnb.SharedKernel.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

using Airbnb.ChatService.Infrastructure.HttpClients;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Airbnb.ChatService.Features.Hubs;

namespace Airbnb.ChatService.Features.Consumers;

public class BookingConfirmedEventConsumer(
    AppDbContext db,
    PropertyServiceClient propertyClient,
    UserServiceClient userClient,
    IHubContext<ChatHub> hubContext,
    ILogger<BookingConfirmedEventConsumer> logger) : IConsumer<BookingConfirmedEvent>
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
            logger.LogInformation("Instant Book detected for Property {PropertyId}, creating new conversation...", message.PropertyId);
            
            // 1. Fetch Property Info to get Title & HostId
            var propertyInfo = await propertyClient.GetPropertyBasicInfoAsync(message.PropertyId, context.CancellationToken);
            if (propertyInfo == null)
            {
                logger.LogWarning("Cannot fetch property {PropertyId}. Aborting conversation creation.", message.PropertyId);
                return;
            }

            // 2. Fetch User Profiles if they don't exist in ChatUsers
            var guestUser = await db.ChatUsers.FindAsync(new object[] { message.UserId }, context.CancellationToken);
            if (guestUser == null)
            {
                var guestProfile = await userClient.GetPublicProfileAsync(message.UserId, context.CancellationToken);
                if (guestProfile == null)
                {
                    logger.LogWarning("Cannot fetch guest profile for {UserId}. Aborting conversation creation.", message.UserId);
                    return;
                }
                
                guestUser = new ChatUser
                {
                    UserId = message.UserId,
                    DisplayName = guestProfile.FullName,
                    AvatarUrl = guestProfile.AvatarUrl
                };
                db.ChatUsers.Add(guestUser);
            }

            var hostUser = await db.ChatUsers.FindAsync(new object[] { propertyInfo.HostId }, context.CancellationToken);
            if (hostUser == null)
            {
                var hostProfile = await userClient.GetPublicProfileAsync(propertyInfo.HostId, context.CancellationToken);
                if (hostProfile == null)
                {
                    logger.LogWarning("Cannot fetch host profile for {HostId}. Aborting conversation creation.", propertyInfo.HostId);
                    return;
                }
                
                hostUser = new ChatUser
                {
                    UserId = propertyInfo.HostId,
                    DisplayName = hostProfile.FullName,
                    AvatarUrl = hostProfile.AvatarUrl
                };
                db.ChatUsers.Add(hostUser);
            }

            // 3. Create Conversation
            conversation = new Conversation
            {
                PropertyId = message.PropertyId,
                PropertyTitle = propertyInfo.Title,
                ReservationId = message.BookingId,
                Participants = new List<ConversationParticipant>
                {
                    new ConversationParticipant
                    {
                        UserId = message.UserId,
                        Role = ParticipantRole.Guest
                    },
                    new ConversationParticipant
                    {
                        UserId = propertyInfo.HostId,
                        Role = ParticipantRole.Host
                    }
                }
            };

            db.Conversations.Add(conversation);
        }

        // Kiểm tra tránh trùng lặp event (Idempotency)
        if (conversation.ReservationId == message.BookingId)
        {
            logger.LogInformation("Booking {BookingId} already confirmed for conversation {ConversationId}. Skipping duplicate event.", message.BookingId, conversation.Id);
            return;
        }

        // Cập nhật hoặc ghi đè ReservationId cho Conversation này
        conversation.ReservationId = message.BookingId;

        // Tạo system message
        var systemMsg = new Message
        {
            ConversationId = conversation.Id,
            SenderId = null, // System Message
            MessageType = MessageType.System,
            Content = $" Booking confirmed! {message.CheckIn:MMM dd} - {message.CheckOut:MMM dd, yyyy} • ${message.TotalPrice} 🎉",
        };

        db.Messages.Add(systemMsg);
        
        conversation.LastMessageAt = systemMsg.CreatedAt;

        await db.SaveChangesAsync(context.CancellationToken);

        // 4. Push SignalR tới những người trong Group conversation
        var messagePayload = new 
        {
            systemMsg.Id,
            systemMsg.ConversationId,
            systemMsg.SenderId,
            systemMsg.Content,
            systemMsg.CreatedAt,
            MessageType = systemMsg.MessageType.ToString()
        };

        await hubContext.Clients.Group($"conv_{conversation.Id}").SendAsync("ReceiveMessage", messagePayload, context.CancellationToken);

        // 5. Push SignalR tới user id group của tất cả những người tham gia
        var allUserGroups = conversation.Participants
            .Select(p => $"user_{p.UserId}")
            .ToList();

        if (allUserGroups.Count > 0)
        {
            await hubContext.Clients.Groups(allUserGroups).SendAsync("NewMessage", messagePayload, context.CancellationToken);
        }
    }
}
