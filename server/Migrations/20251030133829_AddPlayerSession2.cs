using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragonGame.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerSession2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerSessions_Stories_StoryId",
                table: "PlayerSessions");

            migrationBuilder.DropIndex(
                name: "IX_PlayerSessions_StoryId",
                table: "PlayerSessions");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "PlayerSessions");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedAt",
                table: "PlayerSessions",
                newName: "CharacterName");

            migrationBuilder.AlterColumn<int>(
                name: "SessionId",
                table: "PlayerSessions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "CharacterDesignJson",
                table: "PlayerSessions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "PlayerSessions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CharacterDesignJson",
                table: "PlayerSessions");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "PlayerSessions");

            migrationBuilder.RenameColumn(
                name: "CharacterName",
                table: "PlayerSessions",
                newName: "LastUpdatedAt");

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionId",
                table: "PlayerSessions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                table: "PlayerSessions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSessions_StoryId",
                table: "PlayerSessions",
                column: "StoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerSessions_Stories_StoryId",
                table: "PlayerSessions",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "StoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
