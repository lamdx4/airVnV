using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.PropertyService.Migrations
{
    /// <inheritdoc />
    public partial class FlattenPricingCapacityAndUpgradeHouseRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "Pricing",
                table: "properties");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.AddColumn<int>(
                name: "capacity_bathroom_count",
                table: "properties",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "capacity_bed_count",
                table: "properties",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "capacity_bedroom_count",
                table: "properties",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "capacity_guest_count",
                table: "properties",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "pricing_base_price",
                table: "properties",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "pricing_cleaning_fee",
                table: "properties",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "pricing_currency_code",
                table: "properties",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "pricing_service_fee",
                table: "properties",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "pricing_weekend_premium_percent",
                table: "properties",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "capacity_bathroom_count",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "capacity_bed_count",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "capacity_bedroom_count",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "capacity_guest_count",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "pricing_base_price",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "pricing_cleaning_fee",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "pricing_currency_code",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "pricing_service_fee",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "pricing_weekend_premium_percent",
                table: "properties");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.AddColumn<string>(
                name: "Capacity",
                table: "properties",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<string>(
                name: "Pricing",
                table: "properties",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");
        }
    }
}
