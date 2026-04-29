using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.PropertyService.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    PricePerNight = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Address_CountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Address_City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Address_StateProvince = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Address_Ward = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Address_StreetLine1 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Address_StreetLine2 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Address_PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Address_Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Address_Longitude = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Properties");
        }
    }
}
