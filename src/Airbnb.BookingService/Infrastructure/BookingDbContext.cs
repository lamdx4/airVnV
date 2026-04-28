using Microsoft.EntityFrameworkCore;
using Airbnb.BookingService.Domain;

namespace Airbnb.BookingService.Infrastructure;

public class BookingDbContext(DbContextOptions<BookingDbContext> options) : DbContext(options)
{
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>().ToTable("Bookings").HasKey(x => x.Id);
        modelBuilder.Entity<Booking>().Property(b => b.Status).HasConversion<string>();
        
        modelBuilder.Entity<ProcessedEvent>().ToTable("ProcessedEvents").HasKey(x => x.EventId);
    }
}
