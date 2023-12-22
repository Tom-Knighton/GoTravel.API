using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GoTravel.API.Migrations
{
    /// <inheritdoc />
    public partial class LineModeFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Flags",
                columns: table => new
                {
                    GLFlagId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Flag = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flags", x => x.GLFlagId);
                });

            migrationBuilder.CreateTable(
                name: "Flags_LineModes",
                columns: table => new
                {
                    FlagsGLFlagId = table.Column<int>(type: "integer", nullable: false),
                    GLLineModeLineModeName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flags_LineModes", x => new { x.FlagsGLFlagId, x.GLLineModeLineModeName });
                    table.ForeignKey(
                        name: "FK_Flags_LineModes_Flags_FlagsGLFlagId",
                        column: x => x.FlagsGLFlagId,
                        principalTable: "Flags",
                        principalColumn: "GLFlagId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Flags_LineModes_LineMode_GLLineModeLineModeName",
                        column: x => x.GLLineModeLineModeName,
                        principalTable: "LineMode",
                        principalColumn: "LineModeName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Flags_LineModes_GLLineModeLineModeName",
                table: "Flags_LineModes",
                column: "GLLineModeLineModeName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Flags_LineModes");

            migrationBuilder.DropTable(
                name: "Flags");
        }
    }
}
