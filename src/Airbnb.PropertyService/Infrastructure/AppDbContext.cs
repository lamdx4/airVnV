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
        ChangeTracker.StateChanged += OnEntityStateChanged;
    }

    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<PropertyAvailability> PropertyAvailabilities => Set<PropertyAvailability>();
    public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
    public DbSet<Review> Reviews => Set<Review>();

    // Các entity có client-generated GUID key – EF Core hay nhầm state
    private static readonly HashSet<Type> _clientKeyTypes =
    [
        typeof(PropertyAvailability),
        typeof(PropertyImage),
        typeof(Review)
    ];

    /// <summary>
    /// Bắt entities mới (không từ DB) bị nhầm state khi lần đầu được track.
    /// EF thấy GUID key != Guid.Empty → gán Modified/Unchanged thay vì Added.
    /// </summary>
    private void OnEntityTracked(object? sender, Microsoft.EntityFrameworkCore.ChangeTracking.EntityTrackedEventArgs e)
    {
        if (!e.FromQuery
            && e.Entry.State is EntityState.Modified or EntityState.Unchanged
            && _clientKeyTypes.Contains(e.Entry.Entity.GetType()))
        {
            e.Entry.State = EntityState.Added;
        }
    }

    /// <summary>
    /// Bắt entities bị chuyển từ Added → Modified/Unchanged SAU khi đã được track.
    /// Xảy ra khi EF gọi DetectChanges() và re-evaluates lại state.
    /// </summary>
    private void OnEntityStateChanged(object? sender, Microsoft.EntityFrameworkCore.ChangeTracking.EntityStateChangedEventArgs e)
    {
        if (e.OldState == EntityState.Added
            && e.NewState is EntityState.Modified or EntityState.Unchanged
            && _clientKeyTypes.Contains(e.Entry.Entity.GetType()))
        {
            e.Entry.State = EntityState.Added;
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
