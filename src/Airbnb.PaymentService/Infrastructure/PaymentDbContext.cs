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
    public DbSet<Payout> Payouts => Set<Payout>();
    public DbSet<PayoutItem> PayoutItems => Set<PayoutItem>();
    public DbSet<HostBalance> HostBalances => Set<HostBalance>();
    public DbSet<BalanceEntry> BalanceEntries => Set<BalanceEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var hostBalance = modelBuilder.Entity<HostBalance>();
        hostBalance.ToTable("HostBalances").HasKey(x => x.Id);
        hostBalance.Property(x => x.Id).ValueGeneratedNever();
        hostBalance.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        hostBalance.Property(x => x.PendingBalance).HasColumnType("decimal(18,2)");
        hostBalance.Property(x => x.AvailableBalance).HasColumnType("decimal(18,2)");
        hostBalance.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone");
        hostBalance.HasIndex(x => new { x.HostId, x.Currency })
                   .IsUnique()
                   .HasDatabaseName("ix_host_balances_host_currency");

        var entry = modelBuilder.Entity<BalanceEntry>();
        entry.ToTable("BalanceEntries").HasKey(x => x.Id);
        entry.Property(x => x.Id).ValueGeneratedNever();
        entry.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        entry.Property(x => x.Type).HasConversion<string>();
        entry.Property(x => x.PendingDelta).HasColumnType("decimal(18,2)");
        entry.Property(x => x.AvailableDelta).HasColumnType("decimal(18,2)");
        entry.Property(x => x.Note).HasMaxLength(500);
        entry.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");
        entry.HasIndex(x => x.HostId).HasDatabaseName("ix_balance_entries_host");
        entry.HasIndex(x => x.PaymentId).HasDatabaseName("ix_balance_entries_payment");
        entry.HasIndex(x => x.PayoutId).HasDatabaseName("ix_balance_entries_payout");

        var payout = modelBuilder.Entity<Payout>();
        payout.ToTable("Payouts").HasKey(x => x.Id);
        payout.Property(x => x.Id).ValueGeneratedNever();
        payout.Property(x => x.TotalEarnings).HasColumnType("decimal(18,2)");
        payout.Property(x => x.PlatformFee).HasColumnType("decimal(18,2)");
        payout.Property(x => x.PayoutAmount).HasColumnType("decimal(18,2)");
        payout.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        payout.Property(x => x.Status).HasConversion<string>();
        payout.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");
        payout.Property(x => x.ApprovedAt).HasColumnType("timestamp with time zone");
        payout.Property(x => x.CompletedAt).HasColumnType("timestamp with time zone");
        payout.HasIndex(x => x.HostId).HasDatabaseName("ix_payouts_host_id");
        payout.HasIndex(x => x.Status).HasDatabaseName("ix_payouts_status");
        payout.HasMany(x => x.Items)
              .WithOne()
              .HasForeignKey(i => i.PayoutId)
              .OnDelete(DeleteBehavior.Cascade);

        var item = modelBuilder.Entity<PayoutItem>();
        item.ToTable("PayoutItems").HasKey(x => x.Id);
        item.Property(x => x.Id).ValueGeneratedNever();
        item.Property(x => x.BookingTotal).HasColumnType("decimal(18,2)");
        item.Property(x => x.ServiceFee).HasColumnType("decimal(18,2)");
        item.Property(x => x.HostEarning).HasColumnType("decimal(18,2)");
        item.Property(x => x.PropertyTitle).HasMaxLength(500).IsRequired();
        item.Property(x => x.GuestName).HasMaxLength(200).IsRequired();
        item.HasIndex(x => x.PayoutId).HasDatabaseName("ix_payout_items_payout_id");
        item.HasIndex(x => x.BookingId).HasDatabaseName("ix_payout_items_booking_id");


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
