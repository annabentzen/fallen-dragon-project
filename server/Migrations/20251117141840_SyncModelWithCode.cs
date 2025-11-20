using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DragonGame.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelWithCode : Migration
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

            migrationBuilder.DeleteData(
                table: "Choices",
                keyColumn: "ChoiceId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Choices",
                keyColumn: "ChoiceId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Acts",
                keyColumn: "ActId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Stories",
                keyColumn: "StoryId",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CharacterPoses",
                columns: new[] { "Id", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 1, "pose1.png", "Standing" },
                    { 2, "pose2.png", "Fighting" },
                    { 3, "pose3.png", "Flying" }
                });

            migrationBuilder.InsertData(
                table: "Stories",
                columns: new[] { "StoryId", "Title" },
                values: new object[] { 1, "Fallen Dragon" });

            migrationBuilder.InsertData(
                table: "Acts",
                columns: new[] { "ActId", "ActNumber", "IsEnding", "StoryId", "Text" },
                values: new object[] { 1, 1, false, 1, "The dragon awakens..." });

            migrationBuilder.InsertData(
                table: "Choices",
                columns: new[] { "ChoiceId", "ActId", "NextActNumber", "Text" },
                values: new object[,]
                {
                    { 1, 1, 2, "Go left" },
                    { 2, 1, 3, "Go right" }
                });
        }
    }
}
