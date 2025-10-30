using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragonGame.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerSessions",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CurrentActNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StoryId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSessions", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_PlayerSessions_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "StoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSessions_StoryId",
                table: "PlayerSessions",
                column: "StoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerSessions");
        }
    }
}
