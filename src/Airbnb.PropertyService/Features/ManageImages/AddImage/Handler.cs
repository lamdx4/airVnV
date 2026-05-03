using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Domain.Entities;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Features.ManageImages.AddImage;

public sealed class Handler(AppDbContext db)
    : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var property = await db.Properties
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == req.PropertyId && p.HostId == req.RequesterId, ct)
            ?? throw new NotFoundException("Property not found or access denied.");

        var image = PropertyImage.Create(
            property.Id,
            req.RequesterId,
            new Uri(req.Url),
            req.Type,
            req.DisplayOrder);

        property.AddImage(image);
        await db.SaveChangesAsync(ct);

        return new Response(image.Id);
    }
}
