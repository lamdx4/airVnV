using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Airbnb.PropertyService.Domain;

namespace Airbnb.PropertyService.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Property> Properties => Set<Property>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PropertyConfiguration());
    }
}

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Properties");
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name).IsRequired().HasMaxLength(255);
        builder.Property(p => p.Description).HasMaxLength(2000);
        builder.Property(p => p.PricePerNight).HasColumnType("decimal(18,2)");
        // Sử dụng OwnsOne để map Value Object vào cùng bảng Properties
        builder.OwnsOne(p => p.Address, a =>
        {
            a.Property(x => x.CountryCode).IsRequired().HasMaxLength(2);
            a.Property(x => x.City).IsRequired().HasMaxLength(100);
            a.Property(x => x.StateProvince).HasMaxLength(100);
            a.Property(x => x.Ward).HasMaxLength(100);
            a.Property(x => x.StreetLine1).IsRequired().HasMaxLength(255);
            a.Property(x => x.StreetLine2).HasMaxLength(255);
            a.Property(x => x.PostalCode).HasMaxLength(20);
            a.Property(x => x.Latitude).IsRequired();
            a.Property(x => x.Longitude).IsRequired();
        });
        
        // EF Core mapping for private setters
        builder.Property(p => p.CreatedAt).HasColumnType("timestamp with time zone");
    }
}
