using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.ChatService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChatUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutboxMessage_InboxState_InboxMessageId_InboxConsumerId",
                table: "OutboxMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_OutboxMessage_OutboxState_OutboxId",
                table: "OutboxMessage");

            migrationBuilder.DropIndex(
                name: "idx_messages_conversation_created",
                table: "Messages");

            migrationBuilder.CreateTable(
                name: "ChatUsers",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatUsers", x => x.UserId);
                });

            migrationBuilder.Sql(@"
                INSERT INTO ""ChatUsers"" (""UserId"", ""DisplayName"", ""AvatarUrl"")
                SELECT DISTINCT ON (""UserId"") ""UserId"", ""DisplayName"", ""AvatarUrl""
                FROM ""ConversationParticipants"";
            ");

            migrationBuilder.CreateIndex(
                name: "idx_messages_conversation_id",
                table: "Messages",
                columns: new[] { "ConversationId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_ChatUsers_UserId",
                table: "ConversationParticipants",
                column: "UserId",
                principalTable: "ChatUsers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipants_ChatUsers_UserId",
                table: "ConversationParticipants");

            migrationBuilder.DropTable(
                name: "ChatUsers");

            migrationBuilder.DropIndex(
                name: "idx_messages_conversation_id",
                table: "Messages");

            migrationBuilder.CreateIndex(
                name: "idx_messages_conversation_created",
                table: "Messages",
                columns: new[] { "ConversationId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxMessage_InboxState_InboxMessageId_InboxConsumerId",
                table: "OutboxMessage",
                columns: new[] { "InboxMessageId", "InboxConsumerId" },
                principalTable: "InboxState",
                principalColumns: new[] { "MessageId", "ConsumerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxMessage_OutboxState_OutboxId",
                table: "OutboxMessage",
                column: "OutboxId",
                principalTable: "OutboxState",
                principalColumn: "OutboxId");
        }
    }
}
