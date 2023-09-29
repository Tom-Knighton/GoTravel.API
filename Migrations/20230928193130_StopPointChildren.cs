using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoLondon.API.Migrations
{
    /// <inheritdoc />
    public partial class StopPointChildren : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EBikesAvailable",
                table: "StopPoint",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BikesAvailable",
                table: "StopPoint",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StopPoint_StopPointParentId",
                table: "StopPoint",
                column: "StopPointParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StopPoint_StopPoint_StopPointParentId",
                table: "StopPoint",
                column: "StopPointParentId",
                principalTable: "StopPoint",
                principalColumn: "StopPointId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StopPoint_StopPoint_StopPointParentId",
                table: "StopPoint");

            migrationBuilder.DropIndex(
                name: "IX_StopPoint_StopPointParentId",
                table: "StopPoint");

            migrationBuilder.AlterColumn<string>(
                name: "EBikesAvailable",
                table: "StopPoint",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BikesAvailable",
                table: "StopPoint",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
