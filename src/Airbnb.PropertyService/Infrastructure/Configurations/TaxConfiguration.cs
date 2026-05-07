using Airbnb.PropertyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Airbnb.PropertyService.Infrastructure.Configurations;

public class TaxConfiguration : IEntityTypeConfiguration<Tax>
{
    public void Configure(EntityTypeBuilder<Tax> builder)
    {
        builder.ToTable("Taxes");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.CountryCode).IsRequired().HasMaxLength(2);
        builder.Property(t => t.Type).IsRequired().HasMaxLength(50);
        builder.Property(t => t.Rate).HasColumnType("decimal(5,4)").IsRequired(); // e.g. 0.1000 for 10%
        builder.Property(t => t.IsActive).IsRequired();

        builder.HasOne<Country>()
               .WithMany(c => c.Taxes)
               .HasForeignKey(t => t.CountryCode)
               .OnDelete(DeleteBehavior.Cascade);

        // Avoid mapping domain events
        builder.Ignore(t => t.DomainEvents);
    }
}
