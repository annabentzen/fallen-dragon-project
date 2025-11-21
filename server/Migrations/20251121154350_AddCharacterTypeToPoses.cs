using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragonGame.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterTypeToPoses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CharacterType",
                table: "CharacterPoses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CharacterPoses",
                keyColumn: "Id",
                keyValue: 1,
                column: "CharacterType",
                value: null);

            migrationBuilder.UpdateData(
                table: "CharacterPoses",
                keyColumn: "Id",
                keyValue: 2,
                column: "CharacterType",
                value: null);

            migrationBuilder.UpdateData(
                table: "CharacterPoses",
                keyColumn: "Id",
                keyValue: 3,
                column: "CharacterType",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CharacterType",
                table: "CharacterPoses");
        }
    }
}
