using Airbnb.PropertyService.Domain.Enums;

namespace Airbnb.PropertyService.Domain.Entities;

public class PropertyImage
{
    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid UploadedBy { get; private set; }
    public Uri Url { get; private set; } = default!;
    public ImageType Type { get; private set; }
    public int DisplayOrder { get; private set; }

    private PropertyImage() { }

    public static PropertyImage Create(Guid propertyId, Guid uploadedBy, Uri url, ImageType type, int order)
    {
        ArgumentNullException.ThrowIfNull(url);
        return new PropertyImage
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            UploadedBy = uploadedBy,
            Url = url,
            Type = type,
            DisplayOrder = order
        };
    }
}
