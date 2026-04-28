using Microsoft.EntityFrameworkCore;
using Airbnb.UserService.Domain;

namespace Airbnb.UserService.Infrastructure;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users").HasKey(x => x.Id);
        modelBuilder.Entity<User>().Property(x => x.FullName).IsRequired().HasMaxLength(255);
        modelBuilder.Entity<User>().Property(x => x.Email).IsRequired().HasMaxLength(255);
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        
        modelBuilder.Entity<User>().Property(p => p.CreatedAt).HasColumnType("timestamp with time zone");
    }
}
