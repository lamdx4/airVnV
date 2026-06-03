using Airbnb.SharedKernel.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Domain;
using Airbnb.SharedKernel.Domain;
using MassTransit;

namespace Airbnb.UserService.Infrastructure;

public class UserDbContext : AppDbContextBase
{
    public UserDbContext(
        DbContextOptions<UserDbContext> options, 
        IIntegrationEventBridge bridge,
        IDomainEventPolicyExecutor policyExecutor) : base(options, bridge, policyExecutor)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.EnableSensitiveDataLogging().LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
    }
    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserLogin> UserLogins => Set<UserLogin>();
    public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();
    public DbSet<KycDocument> KycDocuments => Set<KycDocument>();
    public DbSet<KycDocumentImage> KycDocumentImages => Set<KycDocumentImage>();

    // MassTransit Outbox Entities
    public DbSet<MassTransit.EntityFrameworkCoreIntegration.InboxState> InboxState => Set<MassTransit.EntityFrameworkCoreIntegration.InboxState>();
    public DbSet<MassTransit.EntityFrameworkCoreIntegration.OutboxMessage> OutboxMessage => Set<MassTransit.EntityFrameworkCoreIntegration.OutboxMessage>();
    public DbSet<MassTransit.EntityFrameworkCoreIntegration.OutboxState> OutboxState => Set<MassTransit.EntityFrameworkCoreIntegration.OutboxState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().ToTable("Users").HasKey(x => x.Id);
        modelBuilder.Entity<User>().Property(x => x.Id).ValueGeneratedNever();
        modelBuilder.Entity<User>().Property(x => x.Email).IsRequired().HasMaxLength(255);
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<User>().Property(p => p.CreatedAt).HasColumnType("timestamp with time zone");
        
        // Enum to String
        modelBuilder.Entity<User>().Property(x => x.Role).HasConversion<string>();
        modelBuilder.Entity<User>().Property(x => x.Status).HasConversion<string>();
        modelBuilder.Entity<User>().Property(x => x.LastLoginAt).HasColumnType("timestamp with time zone");
        modelBuilder.Entity<User>().Property(x => x.SuspensionReason).HasMaxLength(500);
        modelBuilder.Entity<User>().Property(x => x.BanReason).HasMaxLength(500);

        modelBuilder.Entity<UserProfile>().ToTable("UserProfiles").HasKey(x => x.UserId);
        modelBuilder.Entity<UserProfile>().Property(x => x.UserId).ValueGeneratedNever();
        modelBuilder.Entity<UserProfile>().Property(x => x.FullName).IsRequired().HasMaxLength(255);

        modelBuilder.Entity<UserProfile>()
            .HasOne<User>()
            .WithOne(u => u.Profile)
            .HasForeignKey<UserProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserLogin>().ToTable("UserLogins").HasKey(x => x.Id);
        modelBuilder.Entity<UserLogin>().Property(x => x.Id).ValueGeneratedNever();
        modelBuilder.Entity<UserLogin>().HasIndex(x => new { x.Provider, x.ProviderKey }).IsUnique();
        
        // Enum to String
        modelBuilder.Entity<UserLogin>().Property(x => x.Provider).HasConversion<string>();

        modelBuilder.Entity<UserLogin>()
            .HasOne(x => x.User)
            .WithMany(u => u.Logins)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRefreshToken>().ToTable("UserRefreshTokens").HasKey(x => x.Id);
        modelBuilder.Entity<UserRefreshToken>().Property(x => x.Id).ValueGeneratedNever();
        modelBuilder.Entity<UserRefreshToken>().Property(p => p.ExpiresAt).HasColumnType("timestamp with time zone");
        modelBuilder.Entity<UserRefreshToken>().Property(p => p.CreatedAt).HasColumnType("timestamp with time zone");
        modelBuilder.Entity<UserRefreshToken>().Property(p => p.LoginAt).HasColumnType("timestamp with time zone");
        modelBuilder.Entity<UserRefreshToken>().Property(p => p.RevokedAt).HasColumnType("timestamp with time zone");

        modelBuilder.Entity<UserRefreshToken>()
            .HasOne(x => x.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // KYC Documents
        modelBuilder.Entity<KycDocument>().ToTable("KycDocuments").HasKey(x => x.Id);
        modelBuilder.Entity<KycDocument>().Property(x => x.Id).ValueGeneratedNever();
        modelBuilder.Entity<KycDocument>().Property(x => x.Status).HasConversion<string>();
        modelBuilder.Entity<KycDocument>().Property(x => x.DocumentType).HasMaxLength(50);
        modelBuilder.Entity<KycDocument>().Property(x => x.RejectionReason).HasMaxLength(500);
        modelBuilder.Entity<KycDocument>().Property(x => x.SubmittedAt).HasColumnType("timestamp with time zone");
        modelBuilder.Entity<KycDocument>().Property(x => x.ReviewedAt).HasColumnType("timestamp with time zone");

        modelBuilder.Entity<KycDocument>()
            .HasOne(x => x.User)
            .WithMany(u => u.KycDocuments)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // KYC Document Images
        modelBuilder.Entity<KycDocumentImage>().ToTable("KycDocumentImages").HasKey(x => x.Id);
        modelBuilder.Entity<KycDocumentImage>().Property(x => x.Id).ValueGeneratedNever();
        modelBuilder.Entity<KycDocumentImage>().Property(x => x.ImageUrl).IsRequired().HasMaxLength(500);
        modelBuilder.Entity<KycDocumentImage>().Property(x => x.Label).HasMaxLength(100);

        modelBuilder.Entity<KycDocumentImage>()
            .HasOne(x => x.KycDocument)
            .WithMany(d => d.Images)
            .HasForeignKey(x => x.KycDocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
