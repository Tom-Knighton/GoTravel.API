using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoTravel.API.Migrations
{
    /// <inheritdoc />
    public partial class UserPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserPoints",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserPointsAudit",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PointsAtAdd = table.Column<int>(type: "integer", nullable: false),
                    PointsAdded = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPointsAudit", x => new { x.UserId, x.UpdatedAt });
                    table.ForeignKey(
                        name: "FK_UserPointsAudit_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPointsAudit");

            migrationBuilder.DropColumn(
                name: "UserPoints",
                table: "User");
        }
    }
}
