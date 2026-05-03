using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Domain.Entities;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.Infrastructure.Media;

namespace Airbnb.PropertyService.Features.ManageImages.AddImage;

public sealed class Handler(AppDbContext db, IMediaProvider mediaProvider)
    : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        // --- TẦNG 1: PRE-VALIDATION ---
        // Check property existence and ownership before uploading
        var property = await db.Properties
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == req.PropertyId && p.HostId == req.RequesterId, ct)
            ?? throw new NotFoundException("Property not found or access denied.");

        // --- THỰC HIỆN UPLOAD ---
        using var stream = req.File.OpenReadStream();
        var uploadResult = await mediaProvider.UploadAsync(stream, req.File.FileName, "properties", ct);

        try
        {
            // --- DB TRANSACTION ---
            var image = PropertyImage.Create(
                property.Id,
                req.RequesterId,
                uploadResult.Url,
                uploadResult.PublicId,
                req.Type,
                req.DisplayOrder);

            property.AddImage(image);
            await db.SaveChangesAsync(ct);

            return new Response(image.Id, image.Url.ToString());
        }
        catch (Exception)
        {
            // Rollback on Cloudinary if DB save fails
            await mediaProvider.DeleteAsync(uploadResult.PublicId, ct);
            throw; // Re-throw to return 500 error to client
        }
    }
}
