using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investor.Core.Migrations
{
    /// <inheritdoc />
    public partial class createTableChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EvaluationUsers",
                table: "EvaluationUsers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "EvaluationUsers");

            migrationBuilder.AddColumn<string>(
                name: "EvaluationId",
                table: "EvaluationUsers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EvaluationUsers",
                table: "EvaluationUsers",
                column: "EvaluationId");

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    ChatId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    SendUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReceiveUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUpdated = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.ChatId);
                    table.ForeignKey(
                        name: "FK_Chats_Users_ReceiveUserId",
                        column: x => x.ReceiveUserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Chats_Users_SendUserId",
                        column: x => x.SendUserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_ReceiveUserId",
                table: "Chats",
                column: "ReceiveUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_SendUserId",
                table: "Chats",
                column: "SendUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EvaluationUsers",
                table: "EvaluationUsers");

            migrationBuilder.DropColumn(
                name: "EvaluationId",
                table: "EvaluationUsers");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "EvaluationUsers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EvaluationUsers",
                table: "EvaluationUsers",
                column: "Id");
        }
    }
}
