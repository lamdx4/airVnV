using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.Infrastructure.Media;

namespace Airbnb.PropertyService.Features.ManageImages.RemoveImage;

public sealed class Handler(AppDbContext db, IMediaProvider mediaProvider)
    : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var property = await db.Properties
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == req.PropertyId && p.HostId == req.RequesterId, ct)
            ?? throw new NotFoundException("Property not found or access denied.");

        // Find image to get PublicId before removing from collection
        var image = property.Images.FirstOrDefault(i => i.Id == req.ImageId)
            ?? throw new NotFoundException("Image not found.");

        var publicId = image.PublicId;

        property.RemoveImage(req.ImageId);
        await db.SaveChangesAsync(ct);

        // Delete file physically on Cloudinary after DB is clean
        await mediaProvider.DeleteAsync(publicId, ct);

        return new Response(property.Id, "Image removed successfully.");
    }
}
