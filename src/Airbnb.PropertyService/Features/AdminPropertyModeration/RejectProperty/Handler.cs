using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Infrastructure.Messaging;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.AdminPropertyModeration.RejectProperty;

public sealed class Handler(AppDbContext db, DomainEventPublisher publisher)
    : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var property = await db.Properties
            .FirstOrDefaultAsync(p => p.Id == req.PropertyId, ct)
            ?? throw new NotFoundException("Property not found.");

        if (property.Status != Domain.Enums.PropertyStatus.PendingReview)
            throw new BusinessException(
                "Only properties pending review can be rejected.",
                "PROPERTY_NOT_IN_REVIEW");

        property.Archive();
        await publisher.DispatchAsync(property.DomainEvents, ct);
        property.ClearDomainEvents();
        await db.SaveChangesAsync(ct);

        return new Response(property.Id, property.Status.ToString(), DateTimeOffset.UtcNow, "");
    }
}