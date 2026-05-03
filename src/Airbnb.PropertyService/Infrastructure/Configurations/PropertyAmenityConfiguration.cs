using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Airbnb.PropertyService.Domain.Entities;

namespace Airbnb.PropertyService.Infrastructure.Configurations;

public class PropertyAmenityConfiguration : IEntityTypeConfiguration<PropertyAmenity>
{
    public void Configure(EntityTypeBuilder<PropertyAmenity> builder)
    {
        builder.ToTable("property_amenities");
        builder.HasKey(pa => new { pa.PropertyId, pa.AmenityId });
        builder.Property(pa => pa.AdditionalInfo).HasMaxLength(200);
    }
}
