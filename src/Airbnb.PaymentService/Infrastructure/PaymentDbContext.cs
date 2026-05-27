using Airbnb.SharedKernel.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Airbnb.PaymentService.Domain;
using MassTransit;

public class PaymentDbContext(
    DbContextOptions<PaymentDbContext> options,
    IIntegrationEventBridge bridge,
    IDomainEventPolicyExecutor policyExecutor) : AppDbContextBase(options, bridge, policyExecutor)
{
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var payment = modelBuilder.Entity<Payment>();
        payment.ToTable("Payments").HasKey(x => x.Id);
        payment.Property(p => p.Status).HasConversion<string>();
        payment.Property(p => p.Amount).HasColumnType("decimal(18,2)");
        payment.Property(p => p.Currency).HasMaxLength(3).IsRequired();
        payment.Property(p => p.TransactionId).HasMaxLength(255);
        payment.Property(p => p.CreatedAt).HasColumnType("timestamp with time zone");
        payment.Property(p => p.ExpiresAt).HasColumnType("timestamp with time zone");
        payment.Property(p => p.PaymentUrl).HasMaxLength(2048);

        // Filtered Unique Index to prevent multiple Pending payments for the same booking
        payment.HasIndex(p => p.BookingId)
               .IsUnique()
               .HasFilter("\"Status\" = 'Pending'")
               .HasDatabaseName("ix_payments_booking_pending");

        // MassTransit Inbox/Outbox configuration
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
