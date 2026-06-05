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
    public DbSet<Payout> Payouts => Set<Payout>();
    public DbSet<PayoutItem> PayoutItems => Set<PayoutItem>();
    public DbSet<PlatformFeeConfig> PlatformFeeConfigs => Set<PlatformFeeConfig>();
    public DbSet<RefundRecord> RefundRecords => Set<RefundRecord>();

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

        // Payout
        var payout = modelBuilder.Entity<Payout>();
        payout.ToTable("Payouts").HasKey(x => x.Id);
        payout.Property(p => p.Status).HasConversion<string>();
        payout.Property(p => p.TotalEarnings).HasColumnType("decimal(18,2)");
        payout.Property(p => p.PlatformFee).HasColumnType("decimal(18,2)");
        payout.Property(p => p.PayoutAmount).HasColumnType("decimal(18,2)");
        payout.Property(p => p.Currency).HasMaxLength(3).IsRequired();
        payout.Property(p => p.ApprovedAt).HasColumnType("timestamp with time zone");
        payout.Property(p => p.CompletedAt).HasColumnType("timestamp with time zone");
        payout.Property(p => p.CreatedAt).HasColumnType("timestamp with time zone");
        payout.HasMany(p => p.Items)
              .WithOne()
              .HasForeignKey(i => i.PayoutId)
              .OnDelete(DeleteBehavior.Cascade);
        payout.HasIndex(p => p.HostId).HasDatabaseName("ix_payouts_host_id");
        payout.HasIndex(p => p.Status).HasDatabaseName("ix_payouts_status");

        // PayoutItem
        var payoutItem = modelBuilder.Entity<PayoutItem>();
        payoutItem.ToTable("PayoutItems").HasKey(x => x.Id);
        payoutItem.Property(i => i.BookingTotal).HasColumnType("decimal(18,2)");
        payoutItem.Property(i => i.ServiceFee).HasColumnType("decimal(18,2)");
        payoutItem.Property(i => i.HostEarning).HasColumnType("decimal(18,2)");
        payoutItem.Property(i => i.PropertyTitle).HasMaxLength(500).IsRequired();
        payoutItem.Property(i => i.GuestName).HasMaxLength(200).IsRequired();
        payoutItem.HasIndex(i => i.BookingId).HasDatabaseName("ix_payout_items_booking_id");
        payoutItem.HasIndex(i => i.PayoutId).HasDatabaseName("ix_payout_items_payout_id");

        // PlatformFeeConfig
        var feeConfig = modelBuilder.Entity<PlatformFeeConfig>();
        feeConfig.ToTable("PlatformFeeConfigs").HasKey(x => x.Id);
        feeConfig.Property(f => f.FeePercentage).HasColumnType("decimal(5,2)");
        feeConfig.Property(f => f.Description).HasMaxLength(500);
        feeConfig.Property(f => f.PreviousValue).HasColumnType("decimal(5,2)");
        feeConfig.Property(f => f.CreatedAt).HasColumnType("timestamp with time zone");
        feeConfig.HasIndex(f => f.IsActive).HasDatabaseName("ix_platform_fee_config_is_active");

        // RefundRecord
        var refund = modelBuilder.Entity<RefundRecord>();
        refund.ToTable("RefundRecords").HasKey(x => x.Id);
        refund.Property(r => r.Amount).HasColumnType("decimal(18,2)");
        refund.Property(r => r.Reason).HasMaxLength(1000).IsRequired();
        refund.Property(r => r.CreatedAt).HasColumnType("timestamp with time zone");
        refund.HasIndex(r => r.PaymentId).HasDatabaseName("ix_refund_records_payment_id");

        // MassTransit Inbox/Outbox configuration
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
