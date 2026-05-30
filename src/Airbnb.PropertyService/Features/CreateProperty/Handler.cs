using Mediator;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Domain;
using Airbnb.PropertyService.Domain.ValueObjects;
using Airbnb.PropertyService.Infrastructure;
using Airbnb.PropertyService.Infrastructure.Messaging;
using Airbnb.Infrastructure.Media;
using Airbnb.PropertyService.Domain.Enums;
using Airbnb.PropertyService.Domain.Entities;

namespace Airbnb.PropertyService.Features.CreateProperty;

/// <summary>
/// Handler chứa toàn bộ business logic cho CreateProperty.
/// Endpoint không biết gì về DB, Domain, hay Messaging.
/// </summary>
public sealed class Handler(AppDbContext db, DomainEventPublisher publisher, IMediaProvider mediaProvider)
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

            foreach (var amenityId in validAmenities)
            {
                property.AddAmenity(amenityId);
            }
        }

        // Upload images to Cloudinary sequentially to avoid IFormFile concurrent stream read issues & SSL errors
        var uploadResults = new List<(string FileName, Airbnb.Infrastructure.Media.MediaUploadResult Result)>();
        foreach (var file in req.Images)
        {
            using var requestStream = file.OpenReadStream();
            using var memoryStream = new MemoryStream();
            await requestStream.CopyToAsync(memoryStream, ct);
            memoryStream.Position = 0;
            
            var uploadResult = await mediaProvider.UploadAsync(memoryStream, file.FileName, "properties", ct);
            uploadResults.Add((file.FileName, uploadResult));
        }

        try
        {
            int nextOrder = 0;
            foreach (var (fileName, uploadResult) in uploadResults)
            {
                // Match image type by looking up the filename in ImageMetadata, falling back to default logic
                var imageType = data.ImageMetadata?.FirstOrDefault(m => m.FileName == fileName)?.Type 
                    ?? (nextOrder == 0 ? ImageType.Cover : ImageType.Gallery);
                
                var image = PropertyImage.Create(
                    property.Id,
                    req.HostId,
                    uploadResult.Url,
                    uploadResult.PublicId,
                    imageType,
                    nextOrder++);

                property.AddImage(image);
            }

            // Dispatch domain events vào MassTransit Outbox trước SaveChanges
            await publisher.DispatchAsync(property.DomainEvents, ct);
            property.ClearDomainEvents();

            await db.SaveChangesAsync(ct);
            return new Response(property.Id, property.Slug);
        }
        catch (Exception)
        {
            // Cleanup Cloudinary if DB fails
            var deleteTasks = uploadResults.Select(r => mediaProvider.DeleteAsync(r.Result.PublicId, ct));
            await Task.WhenAll(deleteTasks);
            throw;
        }
    }
}
