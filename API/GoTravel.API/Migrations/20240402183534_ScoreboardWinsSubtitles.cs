using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GoTravel.API.Migrations
{
    /// <inheritdoc />
    public partial class ScoreboardWinsSubtitles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScoreboardWins",
                columns: table => new
                {
                    ScoreboardId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    WonAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UUID = table.Column<string>(type: "text", nullable: false),
                    ScoreboardPosition = table.Column<int>(type: "integer", nullable: false),
                    HasSeen = table.Column<bool>(type: "boolean", nullable: false),
                    RewardType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreboardWins", x => new { x.ScoreboardId, x.UserId, x.WonAt });
                    table.ForeignKey(
                        name: "FK_ScoreboardWins_Scoreboards_ScoreboardId",
                        column: x => x.ScoreboardId,
                        principalTable: "Scoreboards",
                        principalColumn: "UUID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoreboardWins_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSubtitles",
                columns: table => new
                {
                    SubtitleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubtitles", x => x.SubtitleId);
                    table.ForeignKey(
                        name: "FK_UserSubtitles_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScoreboardWins_UserId",
                table: "ScoreboardWins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubtitles_UserId",
                table: "UserSubtitles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScoreboardWins");

            migrationBuilder.DropTable(
                name: "UserSubtitles");
        }
    }
}
