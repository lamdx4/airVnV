using Airbnb.SharedKernel.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Airbnb.BookingService.Domain;

public class BookingDbContext(
    DbContextOptions<BookingDbContext> options,
    IIntegrationEventBridge bridge,
    IDomainEventPolicyExecutor policyExecutor) : AppDbContextBase(options, bridge, policyExecutor)
{
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var booking = modelBuilder.Entity<Booking>();
        booking.ToTable("Bookings").HasKey(x => x.Id);
        booking.Property(b => b.Status).HasConversion<string>();
        booking.Property(b => b.CountryCode).HasMaxLength(2).IsRequired();
        booking.Property(b => b.CurrencyCode).HasMaxLength(3).IsRequired();

        // Index chống trùng lịch (Overlap check)
        booking.HasIndex(b => new { b.PropertyId, b.CheckIn, b.CheckOut })
               .HasFilter("\"Status\" != 'Cancelled'")
               .HasDatabaseName("idx_bookings_property_dates");

        // Index tối ưu GetGuestBookings / GetHostBookings
        booking.HasIndex(b => b.GuestId).HasDatabaseName("idx_bookings_guest_id");
        booking.HasIndex(b => b.HostId).HasDatabaseName("idx_bookings_host_id");
        
        modelBuilder.Entity<ProcessedEvent>().ToTable("ProcessedEvents").HasKey(x => x.EventId);
    }
}
