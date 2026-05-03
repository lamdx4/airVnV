using Airbnb.PropertyService.Domain.Entities;
using Airbnb.PropertyService.Domain.Enums;
using Airbnb.PropertyService.Domain.ValueObjects;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.PropertyService.Domain;

/// <summary>
/// Property Aggregate Root – tất cả business logic và invariants nằm ở đây.
/// Không có anemic setter nào được expose ra ngoài.
/// </summary>
public class Property : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid HostId { get; private set; }

    // Core Info
    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string Slug { get; private set; } = default!;

    // Hybrid Address
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string CountryCode { get; private set; } = default!;
    public string? Admin1Code { get; private set; }
    public string? Admin2Code { get; private set; }
    public string DisplayAddress { get; private set; } = default!;
    public AddressRaw AddressRaw { get; private set; } = default!;

    // Core Complex Types
    public Pricing Pricing { get; private set; } = default!;
    public PropertyCapacity Capacity { get; private set; } = default!;
    public HouseRules HouseRules { get; private set; } = default!;

    public PropertyStatus Status { get; private set; }
    public string? SuspensionReason { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private readonly List<PropertyImage> _images = new();
    public IReadOnlyCollection<PropertyImage> Images => _images.AsReadOnly();

    private readonly List<PropertyAmenity> _propertyAmenities = new();
    public IReadOnlyCollection<PropertyAmenity> PropertyAmenities => _propertyAmenities.AsReadOnly();

    private Property() { }

    public static Property Create(
        Guid hostId,
        string title,
        string description,
        string slug,
        double latitude,
        double longitude,
        string countryCode,
        string displayAddress,
        AddressRaw addressRaw,
        Pricing pricing,
        PropertyCapacity capacity,
        HouseRules houseRules,
        string? admin1Code = null,
        string? admin2Code = null)
    {
        if (hostId == Guid.Empty) throw new BusinessException("HostId cannot be empty.", "PROPERTY_HOST_REQUIRED");
        if (string.IsNullOrWhiteSpace(title)) throw new BusinessException("Title cannot be empty.", "PROPERTY_TITLE_REQUIRED");
        if (string.IsNullOrWhiteSpace(slug)) throw new BusinessException("Slug cannot be empty.", "PROPERTY_SLUG_REQUIRED");
        
        return new Property
        {
            Id = Guid.NewGuid(),
            HostId = hostId,
            Title = title,
            Description = description,
            Slug = slug,
            Latitude = latitude,
            Longitude = longitude,
            CountryCode = countryCode.ToUpperInvariant(),
            Admin1Code = admin1Code,
            Admin2Code = admin2Code,
            DisplayAddress = displayAddress,
            AddressRaw = addressRaw,
            Pricing = pricing,
            Capacity = capacity,
            HouseRules = houseRules,
            Status = PropertyStatus.Draft,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }

    public void Submit()
    {
        if (Status != PropertyStatus.Draft)
            throw new BusinessException("Only Draft properties can be submitted for review.", "PROPERTY_INVALID_STATUS");
        
        if (!_images.Any(i => i.Type == ImageType.Cover))
            throw new BusinessException("A cover image is required before submission.", "PROPERTY_COVER_IMAGE_REQUIRED");
            
        Status = PropertyStatus.PendingReview;
        UpdatedAt = DateTimeOffset.UtcNow;
        Raise(new PropertySubmittedEvent(Id, HostId));
    }

    public void Approve()
    {
        if (Status != PropertyStatus.PendingReview)
            throw new BusinessException("Only properties pending review can be approved.", "PROPERTY_NOT_IN_REVIEW");
            
        Status = PropertyStatus.Published;
        UpdatedAt = DateTimeOffset.UtcNow;
        Raise(new PropertyPublishedEvent(Id, HostId, Title, CountryCode, Admin1Code, Admin2Code, Latitude, Longitude));
    }

    public void Suspend(string reason)
    {
        if (Status != PropertyStatus.Published)
            throw new BusinessException("Only published properties can be suspended.", "PROPERTY_NOT_PUBLISHED");
            
        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessException("Suspension reason is required.", "PROPERTY_SUSPENSION_REASON_REQUIRED");
            
        Status = PropertyStatus.Suspended;
        SuspensionReason = reason;
        UpdatedAt = DateTimeOffset.UtcNow;
        Raise(new PropertySuspendedEvent(Id, reason));
    }

    public void Reinstate()
    {
        if (Status != PropertyStatus.Suspended)
            throw new BusinessException("Only suspended properties can be reinstated.", "PROPERTY_NOT_SUSPENDED");
            
        Status = PropertyStatus.Published;
        SuspensionReason = null;
        UpdatedAt = DateTimeOffset.UtcNow;
        Raise(new PropertyReinstatedEvent(Id, HostId));
    }

    public void Archive()
    {
        if (Status is not (PropertyStatus.Published or PropertyStatus.Suspended))
            throw new BusinessException("Only Published or Suspended properties can be archived.", "PROPERTY_INVALID_ARCHIVE_STATUS");
            
        Status = PropertyStatus.Archived;
        UpdatedAt = DateTimeOffset.UtcNow;
        Raise(new PropertyArchivedEvent(Id));
    }

    public void AddAmenity(Guid amenityId, string? additionalInfo = null)
    {
        if (_propertyAmenities.Any(a => a.AmenityId == amenityId))
            throw new BusinessException("Amenity already added to this property.", "PROPERTY_AMENITY_EXISTS");
            
        _propertyAmenities.Add(new PropertyAmenity(Id, amenityId, additionalInfo));
    }

    public void RemoveAmenity(Guid amenityId)
    {
        var amenity = _propertyAmenities.FirstOrDefault(a => a.AmenityId == amenityId)
            ?? throw new BusinessException("Amenity not found on this property.", "PROPERTY_AMENITY_NOT_FOUND");
            
        _propertyAmenities.Remove(amenity);
    }

    public void AddImage(PropertyImage image)
    {
        ArgumentNullException.ThrowIfNull(image);
        
        if (image.Type == ImageType.Cover && _images.Any(i => i.Type == ImageType.Cover))
            throw new BusinessException("A cover image already exists for this property.", "PROPERTY_COVER_IMAGE_EXISTS");
            
        _images.Add(image);
    }

    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId)
            ?? throw new BusinessException("Image not found.", "PROPERTY_IMAGE_NOT_FOUND");

        if (image.Type == ImageType.Cover
            && Status == PropertyStatus.Published
            && _images.Count(i => i.Type == ImageType.Cover) == 1)
            throw new BusinessException("Cannot remove the only cover image of a published property.", "PROPERTY_CANNOT_REMOVE_LAST_COVER");

        _images.Remove(image);
    }

    public void UpdateCoreInfo(
        string? title,
        string? description,
        Pricing? pricing,
        PropertyCapacity? capacity,
        HouseRules? houseRules)
    {
        if (title is not null)
        {
            if (string.IsNullOrWhiteSpace(title)) 
                throw new BusinessException("Title cannot be empty.", "PROPERTY_TITLE_REQUIRED");
            Title = title;
        }
        if (description is not null) Description = description;
        if (pricing is not null) Pricing = pricing;
        if (capacity is not null) Capacity = capacity;
        if (houseRules is not null) HouseRules = houseRules;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void EnsureCanDelete()
    {
        if (Status is not (PropertyStatus.Draft or PropertyStatus.Archived))
            throw new BusinessException("Only Draft or Archived properties can be deleted.", "PROPERTY_CANNOT_BE_DELETED");
    }
}
