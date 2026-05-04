using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.PropertyService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAmenityIconCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("07070707-0707-0707-0707-070707070707"),
                column: "IconCode",
                value: "washing-machine-01");

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("18181818-1818-1818-1818-181818181818"),
                column: "IconCode",
                value: "gymnastics");

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"),
                column: "IconCode",
                value: "wifi-01");

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"),
                column: "IconCode",
                value: "tv-01");

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3"),
                column: "IconCode",
                value: "swimming-pool");

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"),
                column: "IconCode",
                value: "kitchen-01");

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5"),
                column: "IconCode",
                value: "parking-area");

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("f6f6f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6"),
                column: "IconCode",
                value: "snowflake");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("07070707-0707-0707-0707-070707070707"),
                column: "IconCode",
                value: "washer");

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("18181818-1818-1818-1818-181818181818"),
                column: "IconCode",
                value: "gym");

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"),
                column: "IconCode",
                value: "wifi");

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"),
                column: "IconCode",
                value: "tv");

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3"),
                column: "IconCode",
                value: "pool");

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"),
                column: "IconCode",
                value: "kitchen");

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5"),
                column: "IconCode",
                value: "parking");

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("f6f6f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6"),
                column: "IconCode",
                value: "ac");
        }
    }
}
