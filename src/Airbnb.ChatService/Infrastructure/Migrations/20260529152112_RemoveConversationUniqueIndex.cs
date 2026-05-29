using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.ChatService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveConversationUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "uq_conversation_property_no_res",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "uq_conversation_property_res",
                table: "Conversations");

            migrationBuilder.CreateIndex(
                name: "idx_conversation_property",
                table: "Conversations",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "idx_conversation_reservation",
                table: "Conversations",
                column: "ReservationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_conversation_property",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "idx_conversation_reservation",
                table: "Conversations");

            migrationBuilder.CreateIndex(
                name: "uq_conversation_property_no_res",
                table: "Conversations",
                column: "PropertyId",
                unique: true,
                filter: "\"ReservationId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "uq_conversation_property_res",
                table: "Conversations",
                columns: new[] { "PropertyId", "ReservationId" },
                unique: true,
                filter: "\"ReservationId\" IS NOT NULL");
        }
    }
}
