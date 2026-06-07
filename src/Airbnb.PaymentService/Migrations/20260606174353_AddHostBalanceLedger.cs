using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.PaymentService.Migrations
{
    /// <inheritdoc />
    public partial class AddHostBalanceLedger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BalanceEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    PendingDelta = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AvailableDelta = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PayoutId = table.Column<Guid>(type: "uuid", nullable: true),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: true),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HostBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    PendingBalance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AvailableBalance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostBalances", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_balance_entries_host",
                table: "BalanceEntries",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "ix_balance_entries_payment",
                table: "BalanceEntries",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "ix_balance_entries_payout",
                table: "BalanceEntries",
                column: "PayoutId");

            migrationBuilder.CreateIndex(
                name: "ix_host_balances_host_currency",
                table: "HostBalances",
                columns: new[] { "HostId", "Currency" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "BalanceEntries");
            migrationBuilder.DropTable(name: "HostBalances");
        }
    }
}
