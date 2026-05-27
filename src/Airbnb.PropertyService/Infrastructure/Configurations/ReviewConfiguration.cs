using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Airbnb.PropertyService.Domain.Entities;

namespace Airbnb.PropertyService.Infrastructure.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.BookingId).IsRequired();
        builder.Property(r => r.GuestId).IsRequired();
        builder.Property(r => r.Rating).IsRequired();
        builder.Property(r => r.Comment).HasMaxLength(1000);
        builder.Property(r => r.CreatedAt).HasColumnType("timestamp with time zone");

        // Chống spam: 1 Booking chỉ có 1 Review
        builder.HasIndex(r => r.BookingId).IsUnique();
    }
}
