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
    public BookingMode BookingMode { get; private set; }
    public string? SuspensionReason { get; private set; }
    
    // Review Stats
    public int ReviewCount { get; private set; }
    public decimal AverageRating { get; private set; }
    private readonly List<Review> _reviews = new();
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private readonly List<PropertyImage> _images = new();
    public IReadOnlyCollection<PropertyImage> Images => _images.AsReadOnly();

    private readonly List<PropertyAmenity> _propertyAmenities = new();
    public IReadOnlyCollection<PropertyAmenity> PropertyAmenities => _propertyAmenities.AsReadOnly();

    private readonly List<PropertyAvailability> _availabilities = new();
    public IReadOnlyCollection<PropertyAvailability> Availabilities => _availabilities.AsReadOnly();

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
        string? admin2Code = null,
        BookingMode bookingMode = BookingMode.InstantBook)
    {
        if (hostId == Guid.Empty) throw new BusinessException("HostId cannot be empty.", "PROPERTY_HOST_REQUIRED");
        if (string.IsNullOrWhiteSpace(title)) throw new BusinessException("Title cannot be empty.", "PROPERTY_TITLE_REQUIRED");
        if (string.IsNullOrWhiteSpace(slug)) throw new BusinessException("Slug cannot be empty.", "PROPERTY_SLUG_REQUIRED");
        
        return new Property
        {
            Id = Guid.CreateVersion7(),
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
            BookingMode = bookingMode,
            Status = PropertyStatus.Draft,
            ReviewCount = 0,
            AverageRating = 0m,
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
        Version++;
        Raise(new PropertySubmittedEvent(Id, HostId, Version));
    }

    public void Approve()
    {
        if (Status != PropertyStatus.PendingReview)
            throw new BusinessException("Only properties pending review can be approved.", "PROPERTY_NOT_IN_REVIEW");
            
        Version++;
        Raise(new PropertyPublishedEvent(Id, HostId, Title, CountryCode, Admin1Code, Admin2Code, Latitude, Longitude, Version));
    }

    public void Publish()
    {
        if (Status is not (PropertyStatus.Draft or PropertyStatus.PendingReview))
            throw new BusinessException("Only Draft or Pending Review properties can be published.", "PROPERTY_INVALID_STATUS");

        // Strict validation for high-quality listing
        if (string.IsNullOrWhiteSpace(Title) || Title.Length < 10)
            throw new BusinessException("Title must be at least 10 characters long.", "PROPERTY_TITLE_TOO_SHORT");

        if (string.IsNullOrWhiteSpace(Description) || Description.Length < 20)
            throw new BusinessException("Description must be at least 20 characters long.", "PROPERTY_DESCRIPTION_TOO_SHORT");

        if (Pricing.BasePrice <= 0)
            throw new BusinessException("Base price must be greater than 0.", "PROPERTY_PRICE_REQUIRED");

        if (_images.Count < 5)
            throw new BusinessException("At least 5 images are required to publish.", "PROPERTY_MIN_IMAGES_REQUIRED");

        if (!_images.Any(i => i.Type == ImageType.Cover))
            throw new BusinessException("A cover image is required before publishing.", "PROPERTY_COVER_IMAGE_REQUIRED");

        Status = PropertyStatus.Published;
        UpdatedAt = DateTimeOffset.UtcNow;
        Version++;
        Raise(new PropertyPublishedEvent(Id, HostId, Title, CountryCode, Admin1Code, Admin2Code, Latitude, Longitude, Version));
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
        Version++;
        Raise(new PropertySuspendedEvent(Id, reason, Version));
    }

    public void Reinstate()
    {
        if (Status != PropertyStatus.Suspended)
            throw new BusinessException("Only suspended properties can be reinstated.", "PROPERTY_NOT_SUSPENDED");
            
        Status = PropertyStatus.Published;
        SuspensionReason = null;
        UpdatedAt = DateTimeOffset.UtcNow;
        Version++;
        Raise(new PropertyReinstatedEvent(Id, HostId, Version));
    }

    public void Archive()
    {
        if (Status is not (PropertyStatus.Published or PropertyStatus.Suspended))
            throw new BusinessException("Only Published or Suspended properties can be archived.", "PROPERTY_INVALID_ARCHIVE_STATUS");
            
        Status = PropertyStatus.Archived;
        UpdatedAt = DateTimeOffset.UtcNow;
        Version++;
        Raise(new PropertyArchivedEvent(Id, Version));
    }

    public void UpdateStatus(PropertyStatus status)
    {
        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
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

    public void UpdateAmenityInfo(Guid amenityId, string? additionalInfo)
    {
        var amenity = _propertyAmenities.FirstOrDefault(a => a.AmenityId == amenityId)
            ?? throw new BusinessException("Amenity not found on this property.", "PROPERTY_AMENITY_NOT_FOUND");
            
        amenity.UpdateInfo(additionalInfo);
        UpdatedAt = DateTimeOffset.UtcNow;
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
        HouseRules? houseRules,
        BookingMode? bookingMode)
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
        if (bookingMode is not null) BookingMode = bookingMode.Value;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateLocation(
        double latitude,
        double longitude,
        string countryCode,
        string displayAddress,
        AddressRaw addressRaw,
        string? admin1Code = null,
        string? admin2Code = null)
    {
        Latitude = latitude;
        Longitude = longitude;
        CountryCode = countryCode.ToUpperInvariant();
        DisplayAddress = displayAddress;
        AddressRaw = addressRaw;
        Admin1Code = admin1Code;
        Admin2Code = admin2Code;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void EnsureCanDelete()
    {
        if (Status is not (PropertyStatus.Draft or PropertyStatus.Archived))
            throw new BusinessException("Only Draft or Archived properties can be deleted.", "PROPERTY_CANNOT_BE_DELETED");
    }

    public void BlockDates(DateOnly start, DateOnly end, string? note = null)
    {
        if (start < DateOnly.FromDateTime(DateTime.Today))
            throw new BusinessException("Cannot block dates in the past.", "PROPERTY_INVALID_DATE_RANGE");

        _availabilities.Add(PropertyAvailability.Create(Id, start, end, AvailabilityType.Blocked, note));
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RemoveAvailability(Guid availabilityId)
    {
        var item = _availabilities.FirstOrDefault(a => a.Id == availabilityId)
            ?? throw new BusinessException("Availability record not found.", "PROPERTY_AVAILABILITY_NOT_FOUND");
        
        _availabilities.Remove(item);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ReorderImages(List<Features.ManageImages.ReorderImages.ImageOrderUpdate> orders)
    {
        foreach (var order in orders)
        {
            var image = _images.FirstOrDefault(i => i.Id == order.ImageId)
                ?? throw new BusinessException($"Image {order.ImageId} not found.", "PROPERTY_IMAGE_NOT_FOUND");
            
            image.UpdateOrder(order.DisplayOrder);
        }
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void AddReview(Guid bookingId, Guid guestId, int rating, string comment)
    {
        var review = new Review(Id, bookingId, guestId, rating, comment);
        _reviews.Add(review);

        var totalScore = (AverageRating * ReviewCount) + rating;
        ReviewCount++;
        AverageRating = totalScore / ReviewCount;
        
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
