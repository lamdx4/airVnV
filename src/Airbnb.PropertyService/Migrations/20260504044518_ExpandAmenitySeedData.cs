using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Airbnb.PropertyService.Migrations
{
    /// <inheritdoc />
    public partial class ExpandAmenitySeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("07070707-0707-0707-0707-070707070707"));

            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("18181818-1818-1818-1818-181818181818"));

            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5"));

            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("f6f6f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6"));

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"),
                columns: new[] { "IconCode", "Name" },
                values: new object[] { "music-01", "Bluetooth Sound System" });

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3"),
                columns: new[] { "Category", "IconCode", "Name" },
                values: new object[] { "Cooking", "microwave-01", "Microwave" });

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"),
                columns: new[] { "Category", "IconCode", "Name" },
                values: new object[] { "Facilities", "washing-machine-01", "Washing Machine" });

            migrationBuilder.InsertData(
                table: "amenities",
                columns: new[] { "Id", "Category", "IconCode", "Name" },
                values: new object[,]
                {
                    { new Guid("a2a2a2a2-a2a2-a2a2-a2a2-a2a2a2a2a2a2"), "Connectivity", "computer-desk-01", "Dedicated Workspace" },
                    { new Guid("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"), "Entertainment", "tv-01", "HD TV with Netflix" },
                    { new Guid("b3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3"), "Entertainment", "game-controller-01", "Board Games" },
                    { new Guid("c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1"), "Cooking", "kitchen-01", "Fully Equipped Kitchen" },
                    { new Guid("c2c2c2c2-c2c2-c2c2-c2c2-c2c2a2a2a2a2"), "Cooking", "coffee-01", "Coffee Maker" },
                    { new Guid("c4c4c4c4-c4c4-c4c4-c4c4-c4c4c4c4c4c4"), "Cooking", "barbeque", "BBQ Grill" },
                    { new Guid("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1"), "Facilities", "parking-area", "Free Parking on Premises" },
                    { new Guid("d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2"), "Facilities", "swimming-pool", "Private Pool" },
                    { new Guid("d3d3d3d3-d3d3-d3d3-d3d3-d3d3d3d3d3d3"), "Facilities", "gymnastics", "Gym Access" },
                    { new Guid("d5d5d5d5-d5d5-d5d5-d5d5-d5d5d5d5d5d5"), "Facilities", "elevator", "Elevator" },
                    { new Guid("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1"), "Comfort", "snowflake", "Air Conditioning" },
                    { new Guid("e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2"), "Comfort", "fire-01", "Heating System" },
                    { new Guid("e3e3e3e3-e3e3-e3e3-e3e3-e3e3e3e3e3e3"), "Comfort", "bubble-chat", "Essential Toiletries" },
                    { new Guid("f1f1f1f1-f1f1-f1f1-f1f1-f1f1f1f1f1f1"), "Safety", "security-camera-01", "Security Cameras" },
                    { new Guid("f2f2f2f2-f2f2-f2f2-f2f2-f2f2f2f2f2f2"), "Safety", "notification-01", "Smoke Alarm" },
                    { new Guid("f3f3f3f3-f3f3-f3f3-f3f3-f3f3f3f3f3f3"), "Safety", "medicine-bottle", "First Aid Kit" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("a2a2a2a2-a2a2-a2a2-a2a2-a2a2a2a2a2a2"));

            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"));

            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("b3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3"));

            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1"));

            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("c2c2c2c2-c2c2-c2c2-c2c2-c2c2a2a2a2a2"));

            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("c4c4c4c4-c4c4-c4c4-c4c4-c4c4c4c4c4c4"));

            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("d1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1"));

            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("d2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2"));

            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("d3d3d3d3-d3d3-d3d3-d3d3-d3d3d3d3d3d3"));

            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("d5d5d5d5-d5d5-d5d5-d5d5-d5d5d5d5d5d5"));

            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1"));

            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2"));

            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("e3e3e3e3-e3e3-e3e3-e3e3-e3e3e3e3e3e3"));

            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("f1f1f1f1-f1f1-f1f1-f1f1-f1f1f1f1f1f1"));

            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("f2f2f2f2-f2f2-f2f2-f2f2-f2f2f2f2f2f2"));

            migrationBuilder.DeleteData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("f3f3f3f3-f3f3-f3f3-f3f3-f3f3f3f3f3f3"));

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"),
                columns: new[] { "IconCode", "Name" },
                values: new object[] { "tv-01", "HD TV with Netflix" });

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3"),
                columns: new[] { "Category", "IconCode", "Name" },
                values: new object[] { "Outdoor", "swimming-pool", "Private Pool" });

            migrationBuilder.UpdateData(
                table: "amenities",
                keyColumn: "Id",
                keyValue: new Guid("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"),
                columns: new[] { "Category", "IconCode", "Name" },
                values: new object[] { "Cooking", "kitchen-01", "Fully Equipped Kitchen" });

            migrationBuilder.InsertData(
                table: "amenities",
                columns: new[] { "Id", "Category", "IconCode", "Name" },
                values: new object[,]
                {
                    { new Guid("07070707-0707-0707-0707-070707070707"), "Facilities", "washing-machine-01", "Washing Machine" },
                    { new Guid("18181818-1818-1818-1818-181818181818"), "Facilities", "gymnastics", "Gym Access" },
                    { new Guid("e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5"), "Facilities", "parking-area", "Free Parking" },
                    { new Guid("f6f6f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6"), "Comfort", "snowflake", "Air Conditioning" }
                });
        }
    }
}
