using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.ChatService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintPropertyReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_conversation_property_reservation_unique",
                table: "Conversations",
                columns: new[] { "PropertyId", "ReservationId" },
                unique: true,
                filter: "\"ReservationId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_conversation_property_reservation_unique",
                table: "Conversations");
        }
    }
}
