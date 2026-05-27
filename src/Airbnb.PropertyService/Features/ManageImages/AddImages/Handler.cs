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

        var uploadResults = new List<Airbnb.Infrastructure.Media.MediaUploadResult>();
        foreach (var file in req.Files)
        {
            using var requestStream = file.OpenReadStream();
            using var memoryStream = new MemoryStream();
            await requestStream.CopyToAsync(memoryStream, ct);
            memoryStream.Position = 0;
            
            var uploadResult = await mediaProvider.UploadAsync(memoryStream, file.FileName, "properties", ct);
            uploadResults.Add(uploadResult);
        }
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
