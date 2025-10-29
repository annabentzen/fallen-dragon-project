using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DragonGame.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stories",
                columns: table => new
                {
                    StoryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stories", x => x.StoryId);
                });

            migrationBuilder.CreateTable(
                name: "Acts",
                columns: table => new
                {
                    ActId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    ActNumber = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acts", x => x.ActId);
                    table.ForeignKey(
                        name: "FK_Acts_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "StoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Choices",
                columns: table => new
                {
                    ChoiceId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    ActId = table.Column<int>(type: "INTEGER", nullable: false),
                    NextActNumber = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Choices", x => x.ChoiceId);
                    table.ForeignKey(
                        name: "FK_Choices_Acts_ActId",
                        column: x => x.ActId,
                        principalTable: "Acts",
                        principalColumn: "ActId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Stories",
                columns: new[] { "StoryId", "Title" },
                values: new object[] { 1, "Fallen Dragon" });

            migrationBuilder.InsertData(
                table: "Acts",
                columns: new[] { "ActId", "ActNumber", "StoryId", "Text" },
                values: new object[] { 1, 1, 1, "The dragon awakens..." });

            migrationBuilder.InsertData(
                table: "Choices",
                columns: new[] { "ChoiceId", "ActId", "NextActNumber", "Text" },
                values: new object[,]
                {
                    { 1, 1, 2, "Go left" },
                    { 2, 1, 3, "Go right" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Acts_StoryId",
                table: "Acts",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Choices_ActId",
                table: "Choices",
                column: "ActId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Choices");

            migrationBuilder.DropTable(
                name: "Acts");

            migrationBuilder.DropTable(
                name: "Stories");
        }
    }
}
