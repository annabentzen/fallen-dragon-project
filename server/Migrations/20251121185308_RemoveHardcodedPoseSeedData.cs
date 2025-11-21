using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DragonGame.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHardcodedPoseSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CharacterPoses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CharacterPoses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CharacterPoses",
                keyColumn: "Id",
                keyValue: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CharacterPoses",
                columns: new[] { "Id", "CharacterType", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 1, null, "pose1.png", "Standing" },
                    { 2, null, "pose2.png", "Fighting" },
                    { 3, null, "pose3.png", "Flying" }
                });
        }
    }
}
