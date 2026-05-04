using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Airbnb.PropertyService.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingTablesAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Properties",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Address_PostalCode",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Address_StateProvince",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Address_StreetLine1",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Address_StreetLine2",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Address_Ward",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "PricePerNight",
                table: "Properties");

            migrationBuilder.RenameTable(
                name: "Properties",
                newName: "properties");

            migrationBuilder.RenameColumn(
                name: "Address_Longitude",
                table: "properties",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "Address_Latitude",
                table: "properties",
                newName: "Latitude");

            migrationBuilder.RenameColumn(
                name: "Address_CountryCode",
                table: "properties",
                newName: "CountryCode");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "properties",
                newName: "Title");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "properties",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<string>(
                name: "AddressRaw",
                table: "properties",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<string>(
                name: "Admin1Code",
                table: "properties",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Admin2Code",
                table: "properties",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Capacity",
                table: "properties",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<string>(
                name: "DisplayAddress",
                table: "properties",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HouseRules",
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

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "properties",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "properties",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SuspensionReason",
                table: "properties",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "properties",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_properties",
                table: "properties",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "admin_divisions",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ParentCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Level = table.Column<short>(type: "smallint", nullable: false),
                    NameLocal = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Aliases = table.Column<string[]>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_divisions", x => x.Code);
                    table.ForeignKey(
                        name: "FK_admin_divisions_admin_divisions_ParentCode",
                        column: x => x.ParentCode,
                        principalTable: "admin_divisions",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "amenities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IconCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_amenities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "property_amenities",
                columns: table => new
                {
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    AmenityId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdditionalInfo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_property_amenities", x => new { x.PropertyId, x.AmenityId });
                    table.ForeignKey(
                        name: "FK_property_amenities_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "property_images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    UploadedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    PublicId = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_property_images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_property_images_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "amenities",
                columns: new[] { "Id", "Category", "IconCode", "Name" },
                values: new object[,]
                {
                    { new Guid("07070707-0707-0707-0707-070707070707"), "Facilities", "washer", "Washing Machine" },
                    { new Guid("18181818-1818-1818-1818-181818181818"), "Facilities", "gym", "Gym Access" },
                    { new Guid("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"), "Connectivity", "wifi", "High-speed Wi-Fi" },
                    { new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"), "Entertainment", "tv", "HD TV with Netflix" },
                    { new Guid("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3"), "Outdoor", "pool", "Private Pool" },
                    { new Guid("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"), "Cooking", "kitchen", "Fully Equipped Kitchen" },
                    { new Guid("e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5"), "Facilities", "parking", "Free Parking" },
                    { new Guid("f6f6f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6"), "Comfort", "ac", "Air Conditioning" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_properties_CountryCode_Admin1Code_Admin2Code",
                table: "properties",
                columns: new[] { "CountryCode", "Admin1Code", "Admin2Code" });

            migrationBuilder.CreateIndex(
                name: "IX_properties_Slug",
                table: "properties",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_admin_divisions_CountryCode_Level",
                table: "admin_divisions",
                columns: new[] { "CountryCode", "Level" });

            migrationBuilder.CreateIndex(
                name: "IX_admin_divisions_ParentCode",
                table: "admin_divisions",
                column: "ParentCode");

            migrationBuilder.CreateIndex(
                name: "IX_property_images_PropertyId_Type",
                table: "property_images",
                columns: new[] { "PropertyId", "Type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_divisions");

            migrationBuilder.DropTable(
                name: "amenities");

            migrationBuilder.DropTable(
                name: "property_amenities");

            migrationBuilder.DropTable(
                name: "property_images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_properties",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "IX_properties_CountryCode_Admin1Code_Admin2Code",
                table: "properties");

            migrationBuilder.DropIndex(
                name: "IX_properties_Slug",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "AddressRaw",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "Admin1Code",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "Admin2Code",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "DisplayAddress",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "HouseRules",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "Pricing",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "SuspensionReason",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "properties");

            migrationBuilder.RenameTable(
                name: "properties",
                newName: "Properties");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "Properties",
                newName: "Address_Longitude");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "Properties",
                newName: "Address_Latitude");

            migrationBuilder.RenameColumn(
                name: "CountryCode",
                table: "Properties",
                newName: "Address_CountryCode");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Properties",
                newName: "Name");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Properties",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000);

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "Properties",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_PostalCode",
                table: "Properties",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_StateProvince",
                table: "Properties",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_StreetLine1",
                table: "Properties",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_StreetLine2",
                table: "Properties",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Ward",
                table: "Properties",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerNight",
                table: "Properties",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Properties",
                table: "Properties",
                column: "Id");
        }
    }
}
