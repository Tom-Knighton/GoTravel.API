using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GoTravel.API.Migrations
{
    /// <inheritdoc />
    public partial class Areas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                table: "LineMode",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrandingColour",
                table: "LineMode",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryColour",
                table: "LineMode",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondaryColour",
                table: "LineMode",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Area",
                columns: table => new
                {
                    AreaId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AreaName = table.Column<string>(type: "text", nullable: false),
                    AreaCatchment = table.Column<Polygon>(type: "geometry", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.AreaId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LineMode_AreaId",
                table: "LineMode",
                column: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_LineMode_Area_AreaId",
                table: "LineMode",
                column: "AreaId",
                principalTable: "Area",
                principalColumn: "AreaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineMode_Area_AreaId",
                table: "LineMode");

            migrationBuilder.DropTable(
                name: "Area");

            migrationBuilder.DropIndex(
                name: "IX_LineMode_AreaId",
                table: "LineMode");

            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "LineMode");

            migrationBuilder.DropColumn(
                name: "BrandingColour",
                table: "LineMode");

            migrationBuilder.DropColumn(
                name: "PrimaryColour",
                table: "LineMode");

            migrationBuilder.DropColumn(
                name: "SecondaryColour",
                table: "LineMode");
        }
    }
}
