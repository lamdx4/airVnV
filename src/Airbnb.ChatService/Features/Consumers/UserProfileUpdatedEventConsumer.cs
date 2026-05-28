using Airbnb.ChatService.Infrastructure;
using Airbnb.SharedKernel.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.ChatService.Features.Consumers;

public class UserProfileUpdatedEventConsumer(AppDbContext db) : IConsumer<UserProfileUpdatedEvent>
{
    public async Task Consume(ConsumeContext<UserProfileUpdatedEvent> context)
    {
        var msg = context.Message;

        // Update all participant records for this user in ChatService
        await db.ConversationParticipants
            .Where(p => p.UserId == msg.UserId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.DisplayName, msg.DisplayName)
                .SetProperty(p => p.AvatarUrl, msg.AvatarUrl),
                context.CancellationToken);
    }
}
