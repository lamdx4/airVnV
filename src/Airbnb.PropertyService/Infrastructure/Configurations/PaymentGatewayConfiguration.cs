using Airbnb.PropertyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Airbnb.PropertyService.Infrastructure.Configurations;

public class PaymentGatewayConfiguration : IEntityTypeConfiguration<PaymentGateway>
{
    public void Configure(EntityTypeBuilder<PaymentGateway> builder)
    {
        builder.ToTable("PaymentGateways");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.CountryCode).IsRequired().HasMaxLength(2);
        builder.Property(p => p.Provider).IsRequired().HasMaxLength(50);
        builder.Property(p => p.SupportedCurrencies).HasColumnType("text[]");
        builder.Property(p => p.IsActive).IsRequired();

        builder.HasOne<Country>()
               .WithMany(c => c.PaymentGateways)
               .HasForeignKey(p => p.CountryCode)
               .OnDelete(DeleteBehavior.Cascade);

        // Avoid mapping domain events
        builder.Ignore(p => p.DomainEvents);
    }
}
