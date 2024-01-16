using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoTravel.API.Migrations
{
    /// <inheritdoc />
    public partial class StopPointInfos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StopPointInfoKeys",
                columns: table => new
                {
                    InfoKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StopPointInfoKeys", x => x.InfoKey);
                });

            migrationBuilder.CreateTable(
                name: "StopPointInfo",
                columns: table => new
                {
                    StopPointId = table.Column<string>(type: "text", nullable: false),
                    KeyId = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StopPointInfo", x => new { x.StopPointId, x.KeyId });
                    table.ForeignKey(
                        name: "FK_StopPointInfo_StopPointInfoKeys_KeyId",
                        column: x => x.KeyId,
                        principalTable: "StopPointInfoKeys",
                        principalColumn: "InfoKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StopPointInfo_StopPoint_StopPointId",
                        column: x => x.StopPointId,
                        principalTable: "StopPoint",
                        principalColumn: "StopPointId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StopPointInfo_KeyId",
                table: "StopPointInfo",
                column: "KeyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StopPointInfo");

            migrationBuilder.DropTable(
                name: "StopPointInfoKeys");
        }
    }
}
