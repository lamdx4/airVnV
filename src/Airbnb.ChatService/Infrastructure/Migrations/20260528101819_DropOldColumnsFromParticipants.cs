using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.ChatService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropOldColumnsFromParticipants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "ConversationParticipants");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ConversationParticipants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "ConversationParticipants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ConversationParticipants",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
