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

        // Cập nhật tất cả bản ghi Participant của user này trong ChatService
        var participants = await db.ConversationParticipants
            .Where(p => p.UserId == msg.UserId)
            .ToListAsync(context.CancellationToken);

        foreach (var p in participants)
        {
            p.DisplayName = msg.DisplayName;
            p.AvatarUrl = msg.AvatarUrl;
        }

        if (participants.Any())
        {
            await db.SaveChangesAsync(context.CancellationToken);
        }
    }
}
