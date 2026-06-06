using Microsoft.EntityFrameworkCore.Migrations;

namespace Airbnb.UserService.Infrastructure.Migrations;

/// <summary>
/// Fixes the admin account by setting a proper BCrypt-hashed password.
/// The previous seed migration stored a plaintext password.
/// BCrypt hash for "Admin@123456" (work factor 11).
/// </summary>
public partial class FixAdminPasswordHash : Migration
{
    private const string AdminEmail = "admin@airbnb.com";
    // BCrypt hash for "Admin@123456" — work factor 11
    private const string BcryptHash = "$2a$11$EixZaYVK1fsbw1ZfbX3OXePaWxn96p36WQoeG6Lruj3vjPGga31lW";

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql($"""
            UPDATE "Users"
            SET "HashedPassword" = '{BcryptHash}'
            WHERE "Email" = '{AdminEmail}';
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql($"""
            UPDATE "Users"
            SET "HashedPassword" = 'Admin@123456'
            WHERE "Email" = '{AdminEmail}';
            """);
    }
}
