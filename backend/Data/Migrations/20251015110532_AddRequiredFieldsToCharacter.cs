using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragonGame.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRequiredFieldsToCharacter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_CharacterPoses_PoseId",
                table: "Characters");

            migrationBuilder.DropTable(
                name: "Power");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_CharacterPoses_PoseId",
                table: "Characters",
                column: "PoseId",
                principalTable: "CharacterPoses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_CharacterPoses_PoseId",
                table: "Characters");

            migrationBuilder.CreateTable(
                name: "Power",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false),
                    Element = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Power", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Power_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Power_CharacterId",
                table: "Power",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_CharacterPoses_PoseId",
                table: "Characters",
                column: "PoseId",
                principalTable: "CharacterPoses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
