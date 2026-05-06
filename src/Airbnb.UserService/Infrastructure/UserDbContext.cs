using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Domain;
using MassTransit;
using Mediator;
using Airbnb.SharedKernel.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Airbnb.UserService.Infrastructure;

public class UserDbContext(DbContextOptions<UserDbContext> options, IServiceProvider serviceProvider) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserLogin> UserLogins => Set<UserLogin>();
    public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users").HasKey(x => x.Id);
        modelBuilder.Entity<User>().Property(x => x.Email).IsRequired().HasMaxLength(255);
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<User>().Property(p => p.CreatedAt).HasColumnType("timestamp with time zone");
        
        // Enum to String
        modelBuilder.Entity<User>().Property(x => x.Role).HasConversion<string>();

        modelBuilder.Entity<UserProfile>().ToTable("UserProfiles").HasKey(x => x.UserId);
        modelBuilder.Entity<UserProfile>().Property(x => x.FullName).IsRequired().HasMaxLength(255);

        modelBuilder.Entity<UserProfile>()
            .HasOne<User>()
            .WithOne(u => u.Profile)
            .HasForeignKey<UserProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserLogin>().ToTable("UserLogins").HasKey(x => x.Id);
        modelBuilder.Entity<UserLogin>().HasIndex(x => new { x.Provider, x.ProviderKey }).IsUnique();
        
        // Enum to String
        modelBuilder.Entity<UserLogin>().Property(x => x.Provider).HasConversion<string>();

        modelBuilder.Entity<UserLogin>()
            .HasOne(x => x.User)
            .WithMany(u => u.Logins)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRefreshToken>().ToTable("UserRefreshTokens").HasKey(x => x.Id);
        modelBuilder.Entity<UserRefreshToken>().Property(p => p.ExpiresAt).HasColumnType("timestamp with time zone");
        modelBuilder.Entity<UserRefreshToken>().Property(p => p.CreatedAt).HasColumnType("timestamp with time zone");
        modelBuilder.Entity<UserRefreshToken>().Property(p => p.LoginAt).HasColumnType("timestamp with time zone");
        modelBuilder.Entity<UserRefreshToken>().Property(p => p.RevokedAt).HasColumnType("timestamp with time zone");

        modelBuilder.Entity<UserRefreshToken>()
            .HasOne(x => x.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Quét tìm aggregate roots
        var aggregateRoots = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        // 2. Lấy toàn bộ events
        var domainEvents = aggregateRoots
            .SelectMany(x => x.DomainEvents)
            .ToList();

        // 3. Clear events để tránh publish trùng
        aggregateRoots.ForEach(x => x.ClearDomainEvents());

        // 4. Publish via Mediator
        var mediator = serviceProvider.GetService<IMediator>();
        if (mediator != null)
        {
            foreach (var domainEvent in domainEvents)
            {
                if (domainEvent is INotification notification)
                {
                    await mediator.Publish(notification, cancellationToken);
                }
            }
        }

        // 5. Save changes
        return await base.SaveChangesAsync(cancellationToken);
    }
}
