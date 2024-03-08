using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoTravel.API.Migrations
{
    /// <inheritdoc />
    public partial class CrowdsourceEmbeddings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float[]>(
                name: "Embeddings",
                table: "Crowdsource",
                type: "real[]",
                nullable: false,
                defaultValue: new float[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Embeddings",
                table: "Crowdsource");
        }
    }
}
