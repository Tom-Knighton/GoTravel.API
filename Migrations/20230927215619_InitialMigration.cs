using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace GoLondon.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "LineMode",
                columns: table => new
                {
                    LineModeName = table.Column<string>(type: "text", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineMode", x => x.LineModeName);
                });

            migrationBuilder.CreateTable(
                name: "StopPoint",
                columns: table => new
                {
                    StopPointId = table.Column<string>(type: "text", nullable: false),
                    StopPointType = table.Column<string>(type: "text", nullable: false),
                    StopPointName = table.Column<string>(type: "text", nullable: false),
                    StopPointCoordinate = table.Column<Point>(type: "geometry", nullable: false),
                    StopPointHub = table.Column<string>(type: "text", nullable: true),
                    StopPointParentId = table.Column<string>(type: "text", nullable: true),
                    BusStopIndicator = table.Column<string>(type: "text", nullable: true),
                    BusStopLetter = table.Column<string>(type: "text", nullable: true),
                    BusStopSMSCode = table.Column<string>(type: "text", nullable: true),
                    BikesAvailable = table.Column<string>(type: "text", nullable: true),
                    EBikesAvailable = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StopPoint", x => x.StopPointId);
                });

            migrationBuilder.CreateTable(
                name: "Line",
                columns: table => new
                {
                    LineId = table.Column<string>(type: "text", nullable: false),
                    LineName = table.Column<string>(type: "text", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LineModeId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Line", x => x.LineId);
                    table.ForeignKey(
                        name: "FK_Line_LineMode_LineModeId",
                        column: x => x.LineModeId,
                        principalTable: "LineMode",
                        principalColumn: "LineModeName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StopPointLine",
                columns: table => new
                {
                    StopPointId = table.Column<string>(type: "text", nullable: false),
                    LineId = table.Column<string>(type: "text", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StopPointLine", x => new { x.StopPointId, x.LineId });
                    table.ForeignKey(
                        name: "FK_StopPointLine_Line_LineId",
                        column: x => x.LineId,
                        principalTable: "Line",
                        principalColumn: "LineId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StopPointLine_StopPoint_StopPointId",
                        column: x => x.StopPointId,
                        principalTable: "StopPoint",
                        principalColumn: "StopPointId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Line_LineModeId",
                table: "Line",
                column: "LineModeId");

            migrationBuilder.CreateIndex(
                name: "IX_StopPointLine_LineId",
                table: "StopPointLine",
                column: "LineId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StopPointLine");

            migrationBuilder.DropTable(
                name: "Line");

            migrationBuilder.DropTable(
                name: "StopPoint");

            migrationBuilder.DropTable(
                name: "LineMode");
        }
    }
}
