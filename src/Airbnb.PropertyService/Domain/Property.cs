using Airbnb.PropertyService.Domain.Entities;
using Airbnb.PropertyService.Domain.Enums;
using Airbnb.PropertyService.Domain.ValueObjects;

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

    // Hybrid Address – 3 concerns rõ ràng:
    // [1] Location: source of truth cho geo-query
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    // [2] Classification: index được, FK -> AdminDivision
    public string CountryCode { get; private set; } = default!;
    public string? Admin1Code { get; private set; }
    public string? Admin2Code { get; private set; }
    // [3] Display: precomputed, FE render thẳng
    public string DisplayAddress { get; private set; } = default!;
    public AddressRaw AddressRaw { get; private set; } = default!; // JSONB

    // Core Complex Types – JSONB
    public Pricing Pricing { get; private set; } = default!;
    public PropertyCapacity Capacity { get; private set; } = default!;
    public HouseRules HouseRules { get; private set; } = default!;

    public PropertyStatus Status { get; private set; }
    public string? SuspensionReason { get; private set; } // Persisted, không mất khi resume

    // DateTimeOffset – timezone-aware cho multi-country
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    // Navigation properties – encapsulated
    private readonly List<PropertyImage> _images = new();
    public IReadOnlyCollection<PropertyImage> Images => _images.AsReadOnly();

    private readonly List<PropertyAmenity> _propertyAmenities = new();
    public IReadOnlyCollection<PropertyAmenity> PropertyAmenities => _propertyAmenities.AsReadOnly();

    // EF Core constructor
    private Property() { }

    // ---- Factory Method ----
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
        if (hostId == Guid.Empty) throw new ArgumentException("HostId cannot be empty.");
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be empty.");
        if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentException("Slug cannot be empty.");
        if (latitude is < -90 or > 90) throw new ArgumentOutOfRangeException(nameof(latitude));
        if (longitude is < -180 or > 180) throw new ArgumentOutOfRangeException(nameof(longitude));
        if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Length != 2)
            throw new ArgumentException("CountryCode must be ISO 3166-1 alpha-2 (2 chars).");

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

    // ---- Status Transitions (guarded) ----
    public void Submit()
    {
        if (Status != PropertyStatus.Draft)
            throw new InvalidOperationException("Only Draft can be submitted for review.");
        if (!_images.Any(i => i.Type == ImageType.Cover))
            throw new InvalidOperationException("A cover image is required before submission.");
        Status = PropertyStatus.PendingReview;
        UpdatedAt = DateTimeOffset.UtcNow;
        Raise(new PropertySubmittedEvent(Id, HostId));
    }

    public void Approve()
    {
        if (Status != PropertyStatus.PendingReview)
            throw new InvalidOperationException("Only PendingReview can be approved.");
        Status = PropertyStatus.Published;
        UpdatedAt = DateTimeOffset.UtcNow;
        Raise(new PropertyPublishedEvent(Id, HostId, Title, CountryCode, Admin1Code, Admin2Code, Latitude, Longitude));
    }

    public void Suspend(string reason)
    {
        if (Status != PropertyStatus.Published)
            throw new InvalidOperationException("Only Published can be suspended.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Suspension reason is required.");
        Status = PropertyStatus.Suspended;
        SuspensionReason = reason;
        UpdatedAt = DateTimeOffset.UtcNow;
        Raise(new PropertySuspendedEvent(Id, reason));
    }

    public void Reinstate()
    {
        if (Status != PropertyStatus.Suspended)
            throw new InvalidOperationException("Only Suspended can be reinstated.");
        Status = PropertyStatus.Published;
        SuspensionReason = null;
        UpdatedAt = DateTimeOffset.UtcNow;
        Raise(new PropertyReinstatedEvent(Id, HostId));
    }

    public void Archive()
    {
        if (Status is not (PropertyStatus.Published or PropertyStatus.Suspended))
            throw new InvalidOperationException("Only Published or Suspended properties can be archived.");
        Status = PropertyStatus.Archived;
        UpdatedAt = DateTimeOffset.UtcNow;
        Raise(new PropertyArchivedEvent(Id));
    }

    // ---- Domain Behaviors ----
    public void UpdatePricing(Pricing newPricing)
    {
        ArgumentNullException.ThrowIfNull(newPricing);
        Pricing = newPricing;
        UpdatedAt = DateTimeOffset.UtcNow;
        Raise(new PricingUpdatedEvent(Id, newPricing.BasePrice, newPricing.CurrencyCode));
    }

    public void AddAmenity(Guid amenityId, string? additionalInfo = null)
    {
        if (_propertyAmenities.Any(a => a.AmenityId == amenityId))
            throw new InvalidOperationException("Amenity already added.");
        _propertyAmenities.Add(new PropertyAmenity(Id, amenityId, additionalInfo));
    }

    public void RemoveAmenity(Guid amenityId)
    {
        var amenity = _propertyAmenities.FirstOrDefault(a => a.AmenityId == amenityId)
            ?? throw new InvalidOperationException("Amenity not found.");
        _propertyAmenities.Remove(amenity);
    }

    public void AddImage(PropertyImage image)
    {
        ArgumentNullException.ThrowIfNull(image);
        if (image.Type == ImageType.Cover && _images.Any(i => i.Type == ImageType.Cover))
            throw new InvalidOperationException("A cover image already exists.");
        _images.Add(image);
    }

    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId)
            ?? throw new InvalidOperationException("Image not found.");

        if (image.Type == ImageType.Cover
            && Status == PropertyStatus.Published
            && _images.Count(i => i.Type == ImageType.Cover) == 1)
            throw new InvalidOperationException("Cannot remove the only cover image of a published property.");

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
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be empty.");
            Title = title;
        }
        if (description is not null) Description = description;
        if (pricing is not null) Pricing = pricing;
        if (capacity is not null) Capacity = capacity;
        if (houseRules is not null) HouseRules = houseRules;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Soft guard – chỉ Draft hoặc Archived mới được xóa.
    /// EF sẽ thực hiện hard delete ở Handler.
    /// </summary>
    public void EnsureCanDelete()
    {
        if (Status is not (PropertyStatus.Draft or PropertyStatus.Archived))
            throw new InvalidOperationException(
                "Only Draft or Archived properties can be deleted.");
    }
}
