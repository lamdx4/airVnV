using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Airbnb.PropertyService.Domain.Entities;
using Airbnb.PropertyService.Domain.Enums;

namespace Airbnb.PropertyService.Infrastructure.Configurations;

public class PropertyImageConfiguration : IEntityTypeConfiguration<PropertyImage>
{
    public void Configure(EntityTypeBuilder<PropertyImage> builder)
    {
        builder.ToTable("property_images");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Url)
               .IsRequired()
               .HasConversion(uri => uri.ToString(), str => new Uri(str))
               .HasMaxLength(2048);

        builder.Property(i => i.Type).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(i => new { i.PropertyId, i.Type });
    }
}
