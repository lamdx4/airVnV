using Airbnb.PropertyService.Domain.Entities;
using Airbnb.PropertyService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Airbnb.PropertyService.Infrastructure.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Countries");
        
        builder.HasKey(c => c.Code);
        builder.Property(c => c.Code).HasMaxLength(2); // ISO 3166-1 alpha-2
        
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.NativeCurrency).IsRequired().HasMaxLength(3); // ISO 4217
        builder.Property(c => c.IsSupported).IsRequired();
        builder.Property(c => c.DefaultLatitude).IsRequired().HasDefaultValue(0.0);
        builder.Property(c => c.DefaultLongitude).IsRequired().HasDefaultValue(0.0);
        builder.Property(c => c.AddressFormConfig)
            .HasColumnType("jsonb")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, PropertyJsonContext.Default.AddressFieldConfigList),
                v => System.Text.Json.JsonSerializer.Deserialize(v, PropertyJsonContext.Default.AddressFieldConfigList)
            );

        // Avoid mapping domain events
        builder.Ignore(c => c.DomainEvents);
    }
}
