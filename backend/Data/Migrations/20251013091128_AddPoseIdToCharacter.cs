using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragonGame.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPoseIdToCharacter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PoseId",
                table: "Characters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CharacterPoses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterPoses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_PoseId",
                table: "Characters",
                column: "PoseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_CharacterPoses_PoseId",
                table: "Characters",
                column: "PoseId",
                principalTable: "CharacterPoses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_CharacterPoses_PoseId",
                table: "Characters");

            migrationBuilder.DropTable(
                name: "CharacterPoses");

            migrationBuilder.DropIndex(
                name: "IX_Characters_PoseId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "PoseId",
                table: "Characters");
        }
    }
}
