using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.PropertyService.Migrations
{
    /// <inheritdoc />
    public partial class CleanGlobalMasterDataAndMigrateToVietnam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_divisions");

            migrationBuilder.DropTable(
                name: "PaymentGateways");

            migrationBuilder.DropTable(
                name: "Taxes");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.AlterColumn<string>(
                name: "Admin2Code",
                table: "properties",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Admin1Code",
                table: "properties",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Admin2Code",
                table: "properties",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Admin1Code",
                table: "properties",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "admin_divisions",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Aliases = table.Column<string[]>(type: "text[]", nullable: false),
                    CountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Level = table.Column<short>(type: "smallint", nullable: false),
                    NameEn = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    NameLocal = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ParentCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
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
                name: "Countries",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    AddressFormConfig = table.Column<string>(type: "jsonb", nullable: true),
                    DefaultLatitude = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    DefaultLongitude = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    IsSupported = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NativeCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "PaymentGateways",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SupportedCurrencies = table.Column<string[]>(type: "text[]", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGateways", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentGateways_Countries_CountryCode",
                        column: x => x.CountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Taxes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(5,4)", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Taxes_Countries_CountryCode",
                        column: x => x.CountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_admin_divisions_CountryCode_Level",
                table: "admin_divisions",
                columns: new[] { "CountryCode", "Level" });

            migrationBuilder.CreateIndex(
                name: "IX_admin_divisions_ParentCode",
                table: "admin_divisions",
                column: "ParentCode");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGateways_CountryCode",
                table: "PaymentGateways",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_CountryCode",
                table: "Taxes",
                column: "CountryCode");
        }
    }
}
