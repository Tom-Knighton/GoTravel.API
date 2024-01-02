using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoTravel.API.Migrations
{
    /// <inheritdoc />
    public partial class LineModeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flags_LineModes_LineMode_GLLineModeLineModeName",
                table: "Flags_LineModes");

            migrationBuilder.DropForeignKey(
                name: "FK_Line_LineMode_LineModeId",
                table: "Line");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LineMode",
                table: "LineMode");

            migrationBuilder.RenameColumn(
                name: "GLLineModeLineModeName",
                table: "Flags_LineModes",
                newName: "GLLineModeLineModeId");

            migrationBuilder.RenameIndex(
                name: "IX_Flags_LineModes_GLLineModeLineModeName",
                table: "Flags_LineModes",
                newName: "IX_Flags_LineModes_GLLineModeLineModeId");

            migrationBuilder.AddColumn<string>(
                name: "LineModeId",
                table: "LineMode",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LineMode",
                table: "LineMode",
                column: "LineModeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flags_LineModes_LineMode_GLLineModeLineModeId",
                table: "Flags_LineModes",
                column: "GLLineModeLineModeId",
                principalTable: "LineMode",
                principalColumn: "LineModeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Line_LineMode_LineModeId",
                table: "Line",
                column: "LineModeId",
                principalTable: "LineMode",
                principalColumn: "LineModeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flags_LineModes_LineMode_GLLineModeLineModeId",
                table: "Flags_LineModes");

            migrationBuilder.DropForeignKey(
                name: "FK_Line_LineMode_LineModeId",
                table: "Line");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LineMode",
                table: "LineMode");

            migrationBuilder.DropColumn(
                name: "LineModeId",
                table: "LineMode");

            migrationBuilder.RenameColumn(
                name: "GLLineModeLineModeId",
                table: "Flags_LineModes",
                newName: "GLLineModeLineModeName");

            migrationBuilder.RenameIndex(
                name: "IX_Flags_LineModes_GLLineModeLineModeId",
                table: "Flags_LineModes",
                newName: "IX_Flags_LineModes_GLLineModeLineModeName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LineMode",
                table: "LineMode",
                column: "LineModeName");

            migrationBuilder.AddForeignKey(
                name: "FK_Flags_LineModes_LineMode_GLLineModeLineModeName",
                table: "Flags_LineModes",
                column: "GLLineModeLineModeName",
                principalTable: "LineMode",
                principalColumn: "LineModeName",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Line_LineMode_LineModeId",
                table: "Line",
                column: "LineModeId",
                principalTable: "LineMode",
                principalColumn: "LineModeName",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
