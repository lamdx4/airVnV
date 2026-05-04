using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Airbnb.PropertyService.Domain.Entities;
using Airbnb.PropertyService.Domain;

namespace Airbnb.PropertyService.Infrastructure.Configurations;

public class PropertyAvailabilityConfiguration : IEntityTypeConfiguration<PropertyAvailability>
{
    public void Configure(EntityTypeBuilder<PropertyAvailability> builder)
    {
        builder.ToTable("property_availabilities");
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Note).HasMaxLength(255);
        
        builder.HasOne<Property>()
            .WithMany(p => p.Availabilities)
            .HasForeignKey(a => a.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
