using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Infrastructure.Messaging;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.ApproveProperty;

public sealed class Handler(AppDbContext db, DomainEventPublisher publisher)
    : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        // TODO: Thêm Admin role check khi Gateway forward role header
        var property = await db.Properties
            .FirstOrDefaultAsync(p => p.Id == req.PropertyId, ct)
            ?? throw new NotFoundException("Property not found.");

        property.Approve();
        await publisher.DispatchAsync(property.DomainEvents, ct);
        property.ClearDomainEvents();
        await db.SaveChangesAsync(ct);

        return new Response(property.Id, property.Status.ToString());
    }
}
