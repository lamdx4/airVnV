using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAggregateVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "UserRefreshTokens",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "UserRefreshTokens",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "UserRefreshTokens");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "UserRefreshTokens");
        }
    }
}
