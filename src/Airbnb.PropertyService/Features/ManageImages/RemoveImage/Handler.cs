using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.ManageImages.RemoveImage;

public sealed class Handler(AppDbContext db)
    : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var property = await db.Properties
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == req.PropertyId && p.HostId == req.RequesterId, ct)
            ?? throw new NotFoundException("Property not found or access denied.");

        property.RemoveImage(req.ImageId);
        await db.SaveChangesAsync(ct);

        return new Response(property.Id, "Image removed successfully.");
    }
}
