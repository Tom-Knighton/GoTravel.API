using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoTravel.API.Migrations
{
    /// <inheritdoc />
    public partial class Scoreboards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Scoreboards",
                columns: table => new
                {
                    UUID = table.Column<string>(type: "text", nullable: false),
                    ScoreboardName = table.Column<string>(type: "text", nullable: false),
                    ScoreboardIconUrl = table.Column<string>(type: "text", nullable: true),
                    ScoreboardDescription = table.Column<string>(type: "text", nullable: false),
                    ActiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    JoinType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scoreboards", x => x.UUID);
                });

            migrationBuilder.CreateTable(
                name: "ScoreboardUsers",
                columns: table => new
                {
                    ScoreboardUUID = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreboardUsers", x => new { x.ScoreboardUUID, x.UserId });
                    table.ForeignKey(
                        name: "FK_ScoreboardUsers_Scoreboards_ScoreboardUUID",
                        column: x => x.ScoreboardUUID,
                        principalTable: "Scoreboards",
                        principalColumn: "UUID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoreboardUsers_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScoreboardUsers_UserId",
                table: "ScoreboardUsers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScoreboardUsers");

            migrationBuilder.DropTable(
                name: "Scoreboards");
        }
    }
}
