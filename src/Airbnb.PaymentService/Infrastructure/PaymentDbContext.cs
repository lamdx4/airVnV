using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;

namespace Airbnb.PaymentService.Infrastructure;

public class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options)
{
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<OutboxEvent> OutboxEvents => Set<OutboxEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>().ToTable("Payments").HasKey(x => x.Id);
        modelBuilder.Entity<Payment>().Property(p => p.Status).HasConversion<string>();
        modelBuilder.Entity<Payment>().Property(p => p.Amount).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Payment>().Property(p => p.CreatedAt).HasColumnType("timestamp with time zone");

        modelBuilder.Entity<OutboxEvent>().ToTable("OutboxEvents").HasKey(x => x.Id);
    }
}
