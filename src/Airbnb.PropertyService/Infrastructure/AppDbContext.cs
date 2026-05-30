using MassTransit;
using Microsoft.EntityFrameworkCore;
using Airbnb.PropertyService.Domain;
using Airbnb.PropertyService.Domain.Entities;
using Airbnb.PropertyService.Infrastructure.Configurations;

namespace Airbnb.PropertyService.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
    {
        ChangeTracker.Tracked += OnEntityTracked;
    }

    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Amenity> Amenities => Set<Amenity>();

    private void OnEntityTracked(object? sender, Microsoft.EntityFrameworkCore.ChangeTracking.EntityTrackedEventArgs e)
    {
        if (!e.FromQuery && (e.Entry.State == EntityState.Modified || e.Entry.State == EntityState.Unchanged))
        {
            if (e.Entry.Entity is PropertyAvailability or PropertyImage or Review)
            {
                e.Entry.State = EntityState.Added;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PropertyConfiguration());
        modelBuilder.ApplyConfiguration(new PropertyImageConfiguration());
        modelBuilder.ApplyConfiguration(new AmenityConfiguration());
        modelBuilder.ApplyConfiguration(new PropertyAmenityConfiguration());
        modelBuilder.ApplyConfiguration(new PropertyAvailabilityConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewConfiguration());

        // MassTransit Inbox/Outbox configuration
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}

