using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Domain;

namespace Airbnb.UserService.Infrastructure;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users").HasKey(x => x.Id);
        modelBuilder.Entity<User>().Property(x => x.Email).IsRequired().HasMaxLength(255);
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<User>().Property(p => p.CreatedAt).HasColumnType("timestamp with time zone");

        modelBuilder.Entity<UserProfile>().ToTable("UserProfiles").HasKey(x => x.UserId);
        modelBuilder.Entity<UserProfile>().Property(x => x.FullName).IsRequired().HasMaxLength(255);

        modelBuilder.Entity<UserProfile>()
            .HasOne<User>()
            .WithOne(u => u.Profile)
            .HasForeignKey<UserProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
