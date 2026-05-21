using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.PropertyService.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DefaultLatitude",
                table: "Countries",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DefaultLongitude",
                table: "Countries",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultLatitude",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "DefaultLongitude",
                table: "Countries");
        }
    }
}
