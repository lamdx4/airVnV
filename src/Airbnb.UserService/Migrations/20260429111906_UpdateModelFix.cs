using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.UserService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLogins_Users_UserId1",
                table: "UserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRefreshTokens_Users_UserId1",
                table: "UserRefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_UserRefreshTokens_UserId1",
                table: "UserRefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_UserLogins_UserId1",
                table: "UserLogins");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserRefreshTokens");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserLogins");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "UserRefreshTokens",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "UserLogins",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshTokens_UserId1",
                table: "UserRefreshTokens",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId1",
                table: "UserLogins",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogins_Users_UserId1",
                table: "UserLogins",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRefreshTokens_Users_UserId1",
                table: "UserRefreshTokens",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
