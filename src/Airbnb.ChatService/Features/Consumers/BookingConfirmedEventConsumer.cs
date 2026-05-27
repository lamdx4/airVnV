using Airbnb.ChatService.Domain;
using Airbnb.ChatService.Infrastructure;
using Airbnb.SharedKernel.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

using Airbnb.ChatService.Infrastructure.HttpClients;
using Microsoft.Extensions.Logging;

namespace Airbnb.ChatService.Features.Consumers;

public class BookingConfirmedEventConsumer(
    AppDbContext db,
    PropertyServiceClient propertyClient,
    UserServiceClient userClient,
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

            // 2. Fetch User Profiles
            var guestProfile = await userClient.GetPublicProfileAsync(message.UserId, context.CancellationToken);
            var hostProfile = await userClient.GetPublicProfileAsync(propertyInfo.HostId, context.CancellationToken);

            if (guestProfile == null || hostProfile == null)
            {
                logger.LogWarning("Cannot fetch user profiles. Aborting conversation creation.");
                return;
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
                        Role = ParticipantRole.Guest,
                        DisplayName = guestProfile.FullName,
                        AvatarUrl = guestProfile.AvatarUrl
                    },
                    new ConversationParticipant
                    {
                        UserId = propertyInfo.HostId,
                        Role = ParticipantRole.Host,
                        DisplayName = hostProfile.FullName,
                        AvatarUrl = hostProfile.AvatarUrl
                    }
                }
            };

            db.Conversations.Add(conversation);
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
