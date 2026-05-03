using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Domain;
using Airbnb.PropertyService.Domain.Entities;
using Airbnb.PropertyService.Infrastructure.Configurations;

namespace Airbnb.PropertyService.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Property> Properties => Set<Property>();
    public DbSet<AdminDivision> AdminDivisions => Set<AdminDivision>();
    public DbSet<Amenity> Amenities => Set<Amenity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PropertyConfiguration());
        modelBuilder.ApplyConfiguration(new AdminDivisionConfiguration());
        modelBuilder.ApplyConfiguration(new PropertyImageConfiguration());
        modelBuilder.ApplyConfiguration(new AmenityConfiguration());
        modelBuilder.ApplyConfiguration(new PropertyAmenityConfiguration());
    }
}
