using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Domain.Entities;
using Airbnb.ServiceDefaults.Infrastructure;
using Airbnb.Infrastructure.Media;
using Airbnb.PropertyService.Domain.Enums;

namespace Airbnb.PropertyService.Features.ManageImages.AddImages;

public sealed class Handler(AppDbContext db, IMediaProvider mediaProvider)
    : ICommandHandler<Request, Response>
{
    public async ValueTask<Response> Handle(Request req, CancellationToken ct)
    {
        var property = await db.Properties
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == req.PropertyId && p.HostId == req.RequesterId, ct)
            ?? throw new NotFoundException("Property not found or access denied.");

        var uploadTasks = req.Files.Select(async file => 
        {
            using var stream = file.OpenReadStream();
            return await mediaProvider.UploadAsync(stream, file.FileName, "properties", ct);
        });

        var uploadResults = await Task.WhenAll(uploadTasks);
        var addedImages = new List<ImageResponse>();

        try
        {
            int nextOrder = property.Images.Any() ? property.Images.Max(i => i.DisplayOrder) + 1 : 0;

            foreach (var uploadResult in uploadResults)
            {
                var image = PropertyImage.Create(
                    property.Id,
                    req.RequesterId,
                    uploadResult.Url,
                    uploadResult.PublicId,
                    req.Type,
                    nextOrder++);

                property.AddImage(image);
                addedImages.Add(new ImageResponse(image.Id, image.Url.ToString()));
            }

            await db.SaveChangesAsync(ct);
            return new Response(addedImages);
        }
        catch (Exception)
        {
            // Cleanup Cloudinary if DB fails
            var deleteTasks = uploadResults.Select(r => mediaProvider.DeleteAsync(r.PublicId, ct));
            await Task.WhenAll(deleteTasks);
            throw;
        }
    }
}
