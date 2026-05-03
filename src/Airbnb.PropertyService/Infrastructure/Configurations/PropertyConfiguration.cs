using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Airbnb.PropertyService.Domain;
using Airbnb.PropertyService.Domain.Enums;
using Airbnb.PropertyService.Domain.ValueObjects;

namespace Airbnb.PropertyService.Infrastructure.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("properties");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title).IsRequired().HasMaxLength(255);
        builder.Property(p => p.Description).HasMaxLength(5000);
        builder.Property(p => p.Slug).IsRequired().HasMaxLength(300);
        builder.HasIndex(p => p.Slug).IsUnique();

        // [2] Classification – FK -> admin_divisions
        builder.Property(p => p.CountryCode).IsRequired().HasMaxLength(2);
        builder.Property(p => p.Admin1Code).HasMaxLength(10);
        builder.Property(p => p.Admin2Code).HasMaxLength(10);
        builder.HasIndex(p => new { p.CountryCode, p.Admin1Code, p.Admin2Code });

        // [3] Display
        builder.Property(p => p.DisplayAddress).IsRequired().HasMaxLength(500);

        // [1] Location – index riêng (PostGIS GIST index sẽ thêm qua migration raw SQL)
        builder.Property(p => p.Latitude).IsRequired();
        builder.Property(p => p.Longitude).IsRequired();

        // JSONB Value Objects
        builder.OwnsOne(p => p.AddressRaw, nav => { nav.ToJson(); });

        builder.OwnsOne(p => p.Pricing, nav =>
        {
            nav.ToJson();
            nav.Property(x => x.CurrencyCode).HasMaxLength(3);
        });

        builder.OwnsOne(p => p.Capacity, nav => { nav.ToJson(); });
        builder.OwnsOne(p => p.HouseRules, nav => { nav.ToJson(); });

        builder.Property(p => p.Status).HasConversion<int>();
        builder.Property(p => p.SuspensionReason).HasMaxLength(500);
        builder.Property(p => p.CreatedAt).HasColumnType("timestamp with time zone");
        builder.Property(p => p.UpdatedAt).HasColumnType("timestamp with time zone");

        // Navigation – PropertyImage
        builder.HasMany(p => p.Images)
               .WithOne()
               .HasForeignKey(img => img.PropertyId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
