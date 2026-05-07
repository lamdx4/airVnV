using Airbnb.PropertyService.Domain.Entities;
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

        // Avoid mapping domain events
        builder.Ignore(c => c.DomainEvents);
    }
}
