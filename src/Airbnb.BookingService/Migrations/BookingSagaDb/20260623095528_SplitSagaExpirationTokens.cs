using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.BookingService.Migrations.BookingSagaDb
{
    /// <inheritdoc />
    public partial class SplitSagaExpirationTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpirationTokenId",
                table: "BookingState",
                newName: "PaymentTimeoutTokenId");

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovalTimeoutTokenId",
                table: "BookingState",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BookingMode",
                table: "BookingState",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "BookingState",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalTimeoutTokenId",
                table: "BookingState");

            migrationBuilder.DropColumn(
                name: "BookingMode",
                table: "BookingState");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "BookingState");

            migrationBuilder.RenameColumn(
                name: "PaymentTimeoutTokenId",
                table: "BookingState",
                newName: "ExpirationTokenId");
        }
    }
}
