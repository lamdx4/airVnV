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

        // [2] Classification – Vietnam only, raw strings
        builder.Property(p => p.CountryCode).IsRequired().HasMaxLength(2);
        builder.Property(p => p.Admin1Code).HasMaxLength(100);
        builder.Property(p => p.Admin2Code).HasMaxLength(100);
        builder.HasIndex(p => new { p.CountryCode, p.Admin1Code, p.Admin2Code });

        // [3] Display
        builder.Property(p => p.DisplayAddress).IsRequired().HasMaxLength(500);

        // [1] Location – index riêng (PostGIS GIST index sẽ thêm qua migration raw SQL)
        builder.Property(p => p.Latitude).IsRequired();
        builder.Property(p => p.Longitude).IsRequired();

        // JSONB Value Objects (Dành cho cấu trúc động và Flexible Rules - Native AOT Safe)
        builder.Property(p => p.AddressRaw)
            .HasColumnType("jsonb")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, PropertyJsonContext.Default.AddressRaw),
                v => System.Text.Json.JsonSerializer.Deserialize<AddressRaw>(v, PropertyJsonContext.Default.AddressRaw) ?? new AddressRaw()
            );

        builder.Property(p => p.HouseRules)
            .HasColumnType("jsonb")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, PropertyJsonContext.Default.HouseRules),
                v => System.Text.Json.JsonSerializer.Deserialize<HouseRules>(v, PropertyJsonContext.Default.HouseRules)!
            );

        // Owned Types phẳng (Dành cho các trường siêu tĩnh, cần Queryability và Index hiệu năng cao)
        builder.OwnsOne(p => p.Pricing, pricing =>
        {
            pricing.Property(x => x.BasePrice).HasColumnName("pricing_base_price").IsRequired();
            pricing.Property(x => x.CurrencyCode).HasColumnName("pricing_currency_code").HasMaxLength(3).IsRequired();
            pricing.Property(x => x.CleaningFee).HasColumnName("pricing_cleaning_fee").IsRequired();
            pricing.Property(x => x.ServiceFee).HasColumnName("pricing_service_fee").IsRequired();
            pricing.Property(x => x.WeekendPremiumPercent).HasColumnName("pricing_weekend_premium_percent").IsRequired();
        });

        builder.OwnsOne(p => p.Capacity, capacity =>
        {
            capacity.Property(x => x.GuestCount).HasColumnName("capacity_guest_count").IsRequired();
            capacity.Property(x => x.BedroomCount).HasColumnName("capacity_bedroom_count").IsRequired();
            capacity.Property(x => x.BedCount).HasColumnName("capacity_bed_count").IsRequired();
            capacity.Property(x => x.BathroomCount).HasColumnName("capacity_bathroom_count").IsRequired();
        });

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
