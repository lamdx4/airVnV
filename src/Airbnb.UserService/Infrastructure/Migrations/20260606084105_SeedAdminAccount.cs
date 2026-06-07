using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminAccount : Migration
    {
        private const string AdminEmail = "admin@airbnb.com";
        private const string AdminPassword = "Admin@123456";
        private const string AdminName = "System Admin";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var adminId = Guid.CreateVersion7().ToString();
            var now = DateTime.UtcNow.ToString("O");
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(AdminPassword);

            migrationBuilder.Sql($"""
                INSERT INTO "Users" ("Id", "Email", "HashedPassword", "Role", "Status", "IsVerified", "CreatedAt", "Version")
                VALUES ('{adminId}', '{AdminEmail}', '{hashedPassword}', 'Admin', 'Active', true, '{now}', 0)
                ON CONFLICT ("Email") DO NOTHING;

                INSERT INTO "UserProfiles" ("UserId", "FullName", "AvatarUrl", "PhoneNumber", "Bio")
                VALUES ('{adminId}', '{AdminName}', NULL, NULL, NULL)
                ON CONFLICT ("UserId") DO NOTHING;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"""
                DELETE FROM "UserProfiles" WHERE "UserId" IN (
                    SELECT "Id" FROM "Users" WHERE "Email" = '{AdminEmail}'
                );
                DELETE FROM "Users" WHERE "Email" = '{AdminEmail}';
                """);
        }
    }
}
