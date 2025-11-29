using System;
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
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Hair = table.Column<string>(type: "TEXT", nullable: true),
                    Face = table.Column<string>(type: "TEXT", nullable: true),
                    Outfit = table.Column<string>(type: "TEXT", nullable: true),
                    PoseId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_CharacterPoses_PoseId",
                        column: x => x.PoseId,
                        principalTable: "CharacterPoses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Acts",
                columns: table => new
                {
                    ActId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    IsEnding = table.Column<bool>(type: "INTEGER", nullable: false),
                    StoryId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acts", x => x.ActId);
                    table.UniqueConstraint("AK_Acts_ActNumber", x => x.ActNumber);
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
                    NextActNumber = table.Column<int>(type: "INTEGER", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "PlayerSessions",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterName = table.Column<string>(type: "TEXT", nullable: false),
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false),
                    StoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentActNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSessions", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_PlayerSessions_Acts_CurrentActNumber",
                        column: x => x.CurrentActNumber,
                        principalTable: "Acts",
                        principalColumn: "ActNumber",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerSessions_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerSessions_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "StoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChoiceHistories",
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
                    table.PrimaryKey("PK_ChoiceHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChoiceHistories_Choices_ChoiceId",
                        column: x => x.ChoiceId,
                        principalTable: "Choices",
                        principalColumn: "ChoiceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChoiceHistories_PlayerSessions_PlayerSessionId",
                        column: x => x.PlayerSessionId,
                        principalTable: "PlayerSessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Acts_StoryId",
                table: "Acts",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_PoseId",
                table: "Characters",
                column: "PoseId");

            migrationBuilder.CreateIndex(
                name: "IX_ChoiceHistories_ChoiceId",
                table: "ChoiceHistories",
                column: "ChoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ChoiceHistories_MadeAt",
                table: "ChoiceHistories",
                column: "MadeAt");

            migrationBuilder.CreateIndex(
                name: "IX_ChoiceHistories_PlayerSessionId",
                table: "ChoiceHistories",
                column: "PlayerSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Choices_ActId",
                table: "Choices",
                column: "ActId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSessions_CharacterId",
                table: "PlayerSessions",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSessions_CurrentActNumber",
                table: "PlayerSessions",
                column: "CurrentActNumber");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSessions_StoryId",
                table: "PlayerSessions",
                column: "StoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChoiceHistories");

            migrationBuilder.DropTable(
                name: "Choices");

            migrationBuilder.DropTable(
                name: "PlayerSessions");

            migrationBuilder.DropTable(
                name: "Acts");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Stories");

            migrationBuilder.DropTable(
                name: "CharacterPoses");
        }
    }
}
