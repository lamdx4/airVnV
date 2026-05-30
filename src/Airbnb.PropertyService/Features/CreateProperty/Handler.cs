using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Domain;
using Airbnb.PropertyService.Domain.ValueObjects;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Infrastructure.Messaging;
using Airbnb.Infrastructure.Media;
using Airbnb.ServiceDefaults.Infrastructure;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Airbnb.PropertyService.Domain.Enums;
using Airbnb.PropertyService.Domain.Entities;

namespace Airbnb.PropertyService.Features.CreateProperty;

/// <summary>
/// Handler chứa toàn bộ business logic cho CreateProperty.
/// Endpoint không biết gì về DB, Domain, hay Messaging.
/// </summary>
public sealed class Handler(
    AppDbContext db,
    DomainEventPublisher publisher,
    IMediaProvider mediaProvider,
    ILogger<Handler> logger)
    : ICommandHandler<CreatePropertyCommand, Response>
{
    public async ValueTask<Response> Handle(CreatePropertyCommand req, CancellationToken ct)
    {
        var data = req.Data;
        string? admin1Code = data.Admin1Code;
        string? admin2Code = data.Admin2Code;

        var pricing = new Pricing(
            data.BasePrice,
            data.CurrencyCode,
            data.CleaningFee,
            data.ServiceFee,
            data.WeekendPremiumPercent);

        var addressRaw = new AddressRaw(
            data.StreetAddress,
            data.Unit,
            data.PostalCode,
            SubDivisions: data.SubDivisions,
            Notes: new AddressNotes());

        var capacity = new PropertyCapacity(
            data.GuestCount,
            data.BedroomCount,
            data.BedCount,
            data.BathroomCount);

        var houseRules = new HouseRules(
            AllowPets: data.AllowPets,
            AllowSmoking: data.AllowSmoking,
            AllowEvents: data.AllowEvents,
            CheckInTime: data.CheckInTime,
            CheckOutTime: data.CheckOutTime,
            FlexibleCheckIn: false,
            FlexibleCheckOut: data.FlexibleCheckOut,
            CustomRules: data.CustomRules);

        var property = Property.Create(
            hostId: req.HostId,
            title: data.Title,
            description: data.Description,
            slug: data.Slug,
            latitude: data.Latitude,
            longitude: data.Longitude,
            countryCode: data.CountryCode,
            displayAddress: data.DisplayAddress,
            addressRaw: addressRaw,
            pricing: pricing,
            capacity: capacity,
            houseRules: houseRules,
            type: data.Type,
            admin1Code: admin1Code,
            admin2Code: admin2Code,
            bookingMode: data.BookingMode);

        db.Properties.Add(property);

        if (data.AmenityIds != null && data.AmenityIds.Any())
        {
            var validAmenities = await db.Amenities
                .Where(a => data.AmenityIds.Contains(a.Id))
                .Select(a => a.Id)
                .ToListAsync(ct);

            if (validAmenities.Count != data.AmenityIds.Count)
            {
                var invalidIds = data.AmenityIds.Except(validAmenities).ToList();
                throw new BusinessException(
                    $"One or more amenity IDs are invalid: {string.Join(", ", invalidIds)}", 
                    "PROPERTY_AMENITY_INVALID");
            }

            foreach (var amenityId in validAmenities)
            {
                property.AddAmenity(amenityId);
            }
        }

        // Read file streams sequentially into memory first to avoid concurrent reading issues on IFormFile streams
        var filesToUpload = new List<(string FileName, MemoryStream Stream)>();
        foreach (var file in req.Images)
        {
            var requestStream = file.OpenReadStream();
            var memoryStream = new MemoryStream();
            await requestStream.CopyToAsync(memoryStream, ct);
            memoryStream.Position = 0;
            filesToUpload.Add((file.FileName, memoryStream));
            await requestStream.DisposeAsync();
        }

        var uploadedFiles = new ConcurrentBag<(string FileName, MediaUploadResult Result)>();

        // Upload to Cloudinary in parallel for blazing-fast performance (approx 5x speedup)
        var uploadTasks = filesToUpload.Select(async item =>
        {
            try
            {
                var uploadResult = await mediaProvider.UploadAsync(item.Stream, item.FileName, "properties", ct);
                uploadedFiles.Add((item.FileName, uploadResult));
                return (FileName: item.FileName, Result: uploadResult);
            }
            finally
            {
                await item.Stream.DisposeAsync();
            }
        });

        List<(string FileName, MediaUploadResult Result)> uploadResults;
        try
        {
            uploadResults = (await Task.WhenAll(uploadTasks)).ToList();
        }
        catch (Exception uploadEx)
        {
            logger.LogError(uploadEx, "Parallel image upload failed. Cleaning up successful uploads to avoid orphan Cloudinary files.");
            foreach (var uploaded in uploadedFiles)
            {
                try
                {
                    await mediaProvider.DeleteAsync(uploaded.Result.PublicId, ct);
                }
                catch (Exception deleteEx)
                {
                    logger.LogError(deleteEx, "Failed to delete orphan file {PublicId} from Cloudinary during upload error cleanup.", uploaded.Result.PublicId);
                }
            }
            throw;
        }

        try
        {
            int nextOrder = 0;
            bool assignedCover = false;
            var imagesToCreate = new List<(Uri Url, string PublicId, ImageType Type, int Order)>();

            foreach (var (fileName, uploadResult) in uploadResults)
            {
                // Normalize filenames by extracting baseline name (stripping paths, quotes, and whitespace)
                var cleanFileName = System.IO.Path.GetFileName(fileName).Trim('"').Trim();
                
                var matchedMetadata = data.ImageMetadata?.FirstOrDefault(m => 
                    System.IO.Path.GetFileName(m.FileName).Trim('"').Trim()
                    .Equals(cleanFileName, StringComparison.OrdinalIgnoreCase));
                
                ImageType imageType;
                if (matchedMetadata != null)
                {
                    imageType = matchedMetadata.Type;
                }
                else
                {
                    // Fallback logic: check if any metadata already specifies a Cover image
                    var hasCoverInMetadata = data.ImageMetadata?.Any(m => m.Type == ImageType.Cover) ?? false;
                    imageType = (nextOrder == 0 && !hasCoverInMetadata) ? ImageType.Cover : ImageType.Gallery;
                }
                
                // Absolute Security Rule: Enforce at most exactly one Cover image
                if (imageType == ImageType.Cover)
                {
                    if (assignedCover)
                    {
                        imageType = ImageType.Gallery; // Auto-demote duplicate covers to gallery
                    }
                    else
                    {
                        assignedCover = true;
                    }
                }
                
                imagesToCreate.Add((uploadResult.Url, uploadResult.PublicId, imageType, nextOrder++));
            }

            // Self-healing guard: if no cover was successfully assigned, promote the first image as the Cover
            if (!assignedCover && imagesToCreate.Count > 0)
            {
                var first = imagesToCreate[0];
                imagesToCreate[0] = (first.Url, first.PublicId, ImageType.Cover, first.Order);
            }

            // Create and add property images
            foreach (var img in imagesToCreate)
            {
                var image = PropertyImage.Create(
                    property.Id,
                    req.HostId,
                    img.Url,
                    img.PublicId,
                    img.Type,
                    img.Order);

                property.AddImage(image);
            }

            // Dispatch domain events vào MassTransit Outbox trước SaveChanges
            await publisher.DispatchAsync(property.DomainEvents, ct);
            property.ClearDomainEvents();

            await db.SaveChangesAsync(ct);
            return new Response(property.Id, property.Slug);
        }
        catch (Exception dbEx)
        {
            logger.LogError(dbEx, "Database save failed for property creation. Initiating Cloudinary cleanup for successfully uploaded images.");
            
            // Clean up Cloudinary images if DB transaction fails
            foreach (var r in uploadResults)
            {
                try
                {
                    await mediaProvider.DeleteAsync(r.Result.PublicId, ct);
                }
                catch (Exception deleteEx)
                {
                    // Log delete failure but do not throw, preserving original dbEx!
                    logger.LogError(deleteEx, "Failed to delete file {PublicId} from Cloudinary during database failure cleanup.", r.Result.PublicId);
                }
            }
            throw; // Re-throw the original dbEx
        }
    }
}
