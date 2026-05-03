using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Airbnb.PropertyService.Domain.Entities;

namespace Airbnb.PropertyService.Infrastructure.Configurations;

public class AdminDivisionConfiguration : IEntityTypeConfiguration<AdminDivision>
{
    public void Configure(EntityTypeBuilder<AdminDivision> builder)
    {
        builder.ToTable("admin_divisions");
        builder.HasKey(a => a.Code);

        builder.Property(a => a.Code).HasMaxLength(10);
        builder.Property(a => a.ParentCode).HasMaxLength(10);
        builder.Property(a => a.CountryCode).IsRequired().HasMaxLength(2);
        builder.Property(a => a.NameLocal).IsRequired().HasMaxLength(255);
        builder.Property(a => a.NameEn).HasMaxLength(255);

        // TEXT[] – Postgres native array
        builder.Property(a => a.Aliases).HasColumnType("text[]");

        // Self-referencing FK
        builder.HasOne<AdminDivision>()
               .WithMany()
               .HasForeignKey(a => a.ParentCode)
               .IsRequired(false);

        builder.HasIndex(a => new { a.CountryCode, a.Level });
    }
}
