using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Domain.Enums;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.AdminDeleteProperty;

public sealed class Handler(AppDbContext db)
    : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        // TODO: Thêm Admin role check khi Gateway forward role header
        var property = await db.Properties
            .FirstOrDefaultAsync(p => p.Id == req.PropertyId, ct)
            ?? throw new NotFoundException("Property not found.");

        // Admin can delete properties in any state (emergency removal)
        // For Published/Suspended properties, this is an emergency action
        db.Properties.Remove(property);
        await db.SaveChangesAsync(ct);

        return new Response(property.Id, "Property deleted by admin.");
    }
}
