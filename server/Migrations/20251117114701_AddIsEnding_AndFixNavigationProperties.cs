using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragonGame.Migrations
{
    /// <inheritdoc />
    public partial class AddIsEnding_AndFixNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NextActNumber",
                table: "Choices",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerSessions_Stories_StoryId",
                table: "PlayerSessions");

            migrationBuilder.DropIndex(
                name: "IX_PlayerSessions_StoryId",
                table: "PlayerSessions");

            migrationBuilder.AlterColumn<int>(
                name: "NextActNumber",
                table: "Choices",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }
    }
}
