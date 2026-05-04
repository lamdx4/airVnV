using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Airbnb.PropertyService.Domain.Entities;

namespace Airbnb.PropertyService.Infrastructure.Configurations;

public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
{
    public void Configure(EntityTypeBuilder<Amenity> builder)
    {
        builder.ToTable("amenities");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Category).IsRequired().HasMaxLength(50);
        builder.Property(a => a.IconCode).HasMaxLength(50);

        // Seed Data
        builder.HasData(
            new { Id = Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"), Name = "High-speed Wi-Fi", Category = "Connectivity", IconCode = "wifi" },
            new { Id = Guid.Parse("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"), Name = "HD TV with Netflix", Category = "Entertainment", IconCode = "tv" },
            new { Id = Guid.Parse("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3"), Name = "Private Pool", Category = "Outdoor", IconCode = "pool" },
            new { Id = Guid.Parse("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"), Name = "Fully Equipped Kitchen", Category = "Cooking", IconCode = "kitchen" },
            new { Id = Guid.Parse("e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5"), Name = "Free Parking", Category = "Facilities", IconCode = "parking" },
            new { Id = Guid.Parse("f6f6f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6"), Name = "Air Conditioning", Category = "Comfort", IconCode = "ac" },
            new { Id = Guid.Parse("07070707-0707-0707-0707-070707070707"), Name = "Washing Machine", Category = "Facilities", IconCode = "washer" },
            new { Id = Guid.Parse("18181818-1818-1818-1818-181818181818"), Name = "Gym Access", Category = "Facilities", IconCode = "gym" }
        );
    }
}
