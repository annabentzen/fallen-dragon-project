using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragonGame.Migrations
{
    /// <inheritdoc />
    public partial class RenameHairFaceOutfitToHeadBody : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Face",
                table: "Characters");

            migrationBuilder.RenameColumn(
                name: "Outfit",
                table: "Characters",
                newName: "Head");

            migrationBuilder.RenameColumn(
                name: "Hair",
                table: "Characters",
                newName: "Body");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Head",
                table: "Characters",
                newName: "Outfit");

            migrationBuilder.RenameColumn(
                name: "Body",
                table: "Characters",
                newName: "Hair");

            migrationBuilder.AddColumn<string>(
                name: "Face",
                table: "Characters",
                type: "TEXT",
                nullable: true);
        }
    }
}
