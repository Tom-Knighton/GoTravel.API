using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace GoTravel.API.Migrations
{
    /// <inheritdoc />
    public partial class UserTripsLineRoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LineRoute",
                columns: table => new
                {
                    LineId = table.Column<string>(type: "text", nullable: false),
                    Direction = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ServiceType = table.Column<string>(type: "text", nullable: false),
                    Route = table.Column<MultiLineString>(type: "geometry", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineRoute", x => new { x.LineId, x.Direction, x.Name });
                    table.ForeignKey(
                        name: "FK_LineRoute_Line_LineId",
                        column: x => x.LineId,
                        principalTable: "Line",
                        principalColumn: "LineId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSavedJourney",
                columns: table => new
                {
                    UUID = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LineString = table.Column<LineString>(type: "geometry", nullable: false),
                    NeedsModeration = table.Column<bool>(type: "boolean", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSavedJourney", x => x.UUID);
                });

            migrationBuilder.CreateTable(
                name: "UserSavedJourneyLine",
                columns: table => new
                {
                    SavedJourneyId = table.Column<string>(type: "text", nullable: false),
                    LineId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSavedJourneyLine", x => new { x.SavedJourneyId, x.LineId });
                    table.ForeignKey(
                        name: "FK_UserSavedJourneyLine_Line_LineId",
                        column: x => x.LineId,
                        principalTable: "Line",
                        principalColumn: "LineId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSavedJourneyLine_UserSavedJourney_SavedJourneyId",
                        column: x => x.SavedJourneyId,
                        principalTable: "UserSavedJourney",
                        principalColumn: "UUID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LineRoute_LineId",
                table: "LineRoute",
                column: "LineId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSavedJourneyLine_LineId",
                table: "UserSavedJourneyLine",
                column: "LineId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineRoute");

            migrationBuilder.DropTable(
                name: "UserSavedJourneyLine");

            migrationBuilder.DropTable(
                name: "UserSavedJourney");
        }
    }
}
