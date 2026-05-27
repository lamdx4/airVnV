using Airbnb.SharedKernel.Domain;

namespace Airbnb.PropertyService.Domain.Entities;

public class Review
{
    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid BookingId { get; private set; }
    public Guid GuestId { get; private set; }
    public int Rating { get; private set; }
    public string Comment { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; private set; }

    private Review() { } // EF Core

    public Review(Guid propertyId, Guid bookingId, Guid guestId, int rating, string comment)
    {
        if (propertyId == Guid.Empty) throw new ArgumentException("PropertyId is required");
        if (bookingId == Guid.Empty) throw new ArgumentException("BookingId is required");
        if (guestId == Guid.Empty) throw new ArgumentException("GuestId is required");
        if (rating < 1 || rating > 5) throw new ArgumentException("Rating must be between 1 and 5");
        
        Id = Guid.CreateVersion7();
        PropertyId = propertyId;
        BookingId = bookingId;
        GuestId = guestId;
        Rating = rating;
        Comment = comment?.Trim() ?? string.Empty;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void Update(int newRating, string newComment)
    {
        if (newRating < 1 || newRating > 5) throw new ArgumentException("Rating must be between 1 and 5");
        
        Rating = newRating;
        Comment = newComment?.Trim() ?? string.Empty;
    }
}
