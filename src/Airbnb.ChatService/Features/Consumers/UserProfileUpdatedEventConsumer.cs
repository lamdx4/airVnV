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
        await db.ChatUsers
            .Where(u => u.UserId == msg.UserId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(u => u.DisplayName, msg.DisplayName)
                .SetProperty(u => u.AvatarUrl, msg.AvatarUrl),
                context.CancellationToken);
    }
}
