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

        // Seed Data - Bộ sưu tập 20 tiện nghi phổ biến nhất
        builder.HasData(
            // Connectivity
            new { Id = Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"), Name = "High-speed Wi-Fi", Category = "Connectivity", IconCode = "wifi-01" },
            new { Id = Guid.Parse("a2a2a2a2-a2a2-a2a2-a2a2-a2a2a2a2a2a2"), Name = "Dedicated Workspace", Category = "Connectivity", IconCode = "computer-desk-01" },

            // Entertainment
            new { Id = Guid.Parse("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"), Name = "HD TV with Netflix", Category = "Entertainment", IconCode = "tv-01" },
            new { Id = Guid.Parse("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"), Name = "Bluetooth Sound System", Category = "Entertainment", IconCode = "music-01" },
            new { Id = Guid.Parse("b3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3"), Name = "Board Games", Category = "Entertainment", IconCode = "game-controller-01" },

            // Cooking & Dining
            new { Id = Guid.Parse("c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1"), Name = "Fully Equipped Kitchen", Category = "Cooking", IconCode = "kitchen-01" },
            new { Id = Guid.Parse("c2c2c2c2-c2c2-c2c2-c2c2-c2c2a2a2a2a2"), Name = "Coffee Maker", Category = "Cooking", IconCode = "coffee-01" },
            new { Id = Guid.Parse("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3"), Name = "Microwave", Category = "Cooking", IconCode = "microwave-01" },
            new { Id = Guid.Parse("c4c4c4c4-c4c4-c4c4-c4c4-c4c4c4c4c4c4"), Name = "BBQ Grill", Category = "Cooking", IconCode = "barbeque" },

            // Facilities
            new { Id = Guid.Parse("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1"), Name = "Free Parking on Premises", Category = "Facilities", IconCode = "parking-area" },
            new { Id = Guid.Parse("d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2"), Name = "Private Pool", Category = "Facilities", IconCode = "swimming-pool" },
            new { Id = Guid.Parse("d3d3d3d3-d3d3-d3d3-d3d3-d3d3d3d3d3d3"), Name = "Gym Access", Category = "Facilities", IconCode = "gymnastics" },
            new { Id = Guid.Parse("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"), Name = "Washing Machine", Category = "Facilities", IconCode = "washing-machine-01" },
            new { Id = Guid.Parse("d5d5d5d5-d5d5-d5d5-d5d5-d5d5d5d5d5d5"), Name = "Elevator", Category = "Facilities", IconCode = "elevator" },

            // Comfort
            new { Id = Guid.Parse("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1"), Name = "Air Conditioning", Category = "Comfort", IconCode = "snowflake" },
            new { Id = Guid.Parse("e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2"), Name = "Heating System", Category = "Comfort", IconCode = "fire-01" },
            new { Id = Guid.Parse("e3e3e3e3-e3e3-e3e3-e3e3-e3e3e3e3e3e3"), Name = "Essential Toiletries", Category = "Comfort", IconCode = "bubble-chat" },

            // Safety
            new { Id = Guid.Parse("f1f1f1f1-f1f1-f1f1-f1f1-f1f1f1f1f1f1"), Name = "Security Cameras", Category = "Safety", IconCode = "security-camera-01" },
            new { Id = Guid.Parse("f2f2f2f2-f2f2-f2f2-f2f2-f2f2f2f2f2f2"), Name = "Smoke Alarm", Category = "Safety", IconCode = "notification-01" },
            new { Id = Guid.Parse("f3f3f3f3-f3f3-f3f3-f3f3-f3f3f3f3f3f3"), Name = "First Aid Kit", Category = "Safety", IconCode = "medicine-bottle" }
        );
    }
}
