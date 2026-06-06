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
    public DbSet<PlatformSetting> PlatformSettings => Set<PlatformSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var setting = modelBuilder.Entity<PlatformSetting>();
        setting.ToTable("PlatformSettings").HasKey(x => x.Id);
        setting.Property(p => p.Id).ValueGeneratedNever();
        setting.Property(p => p.PlatformFeePercent).HasColumnType("decimal(5,2)");
        setting.Property(p => p.MinPayoutAmount).HasColumnType("decimal(18,2)");
        setting.Property(p => p.DefaultCurrency).HasMaxLength(3).IsRequired();
        setting.Property(p => p.UpdatedAt).HasColumnType("timestamp with time zone");
        setting.Property(p => p.UpdatedBy).HasMaxLength(255);

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
