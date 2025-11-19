using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragonGame.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CharacterDesignJson",
                table: "PlayerSessions");

            migrationBuilder.AddColumn<int>(
                name: "CharacterId",
                table: "PlayerSessions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ChoiceHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerSessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ActNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ChoiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    MadeAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChoiceHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChoiceHistory_Choices_ChoiceId",
                        column: x => x.ChoiceId,
                        principalTable: "Choices",
                        principalColumn: "ChoiceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChoiceHistory_PlayerSessions_PlayerSessionId",
                        column: x => x.PlayerSessionId,
                        principalTable: "PlayerSessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSessions_CharacterId",
                table: "PlayerSessions",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ChoiceHistory_ChoiceId",
                table: "ChoiceHistory",
                column: "ChoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ChoiceHistory_PlayerSessionId",
                table: "ChoiceHistory",
                column: "PlayerSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerSessions_Characters_CharacterId",
                table: "PlayerSessions",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerSessions_Characters_CharacterId",
                table: "PlayerSessions");

            migrationBuilder.DropTable(
                name: "ChoiceHistory");

            migrationBuilder.DropIndex(
                name: "IX_PlayerSessions_CharacterId",
                table: "PlayerSessions");

            migrationBuilder.DropColumn(
                name: "CharacterId",
                table: "PlayerSessions");

            migrationBuilder.AddColumn<string>(
                name: "CharacterDesignJson",
                table: "PlayerSessions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
