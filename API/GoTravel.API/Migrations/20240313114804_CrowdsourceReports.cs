using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoTravel.API.Migrations
{
    /// <inheritdoc />
    public partial class CrowdsourceReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CrowdsourceReports",
                columns: table => new
                {
                    UUID = table.Column<string>(type: "text", nullable: false),
                    CrowdsourceId = table.Column<string>(type: "text", nullable: false),
                    ReporterId = table.Column<string>(type: "text", nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReportText = table.Column<string>(type: "text", nullable: false),
                    Handled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrowdsourceReports", x => x.UUID);
                    table.ForeignKey(
                        name: "FK_CrowdsourceReports_User_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CrowdsourceReports_CrowdsourceId",
                table: "CrowdsourceReports",
                column: "CrowdsourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CrowdsourceReports_ReporterId",
                table: "CrowdsourceReports",
                column: "ReporterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrowdsourceReports");
        }
    }
}
