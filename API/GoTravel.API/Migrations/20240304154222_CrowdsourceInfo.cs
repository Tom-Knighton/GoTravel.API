using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoTravel.API.Migrations
{
    /// <inheritdoc />
    public partial class CrowdsourceInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Crowdsource",
                columns: table => new
                {
                    UUID = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<string>(type: "text", nullable: false),
                    FreeText = table.Column<string>(type: "text", nullable: true),
                    IsDelayed = table.Column<bool>(type: "boolean", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpectedEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SubmittedById = table.Column<string>(type: "text", nullable: false),
                    NeedsReview = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crowdsource", x => x.UUID);
                    table.ForeignKey(
                        name: "FK_Crowdsource_User_SubmittedById",
                        column: x => x.SubmittedById,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CrowdsourceVotes",
                columns: table => new
                {
                    CrowdsourceId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    VoteType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrowdsourceVotes", x => new { x.CrowdsourceId, x.UserId });
                    table.ForeignKey(
                        name: "FK_CrowdsourceVotes_Crowdsource_CrowdsourceId",
                        column: x => x.CrowdsourceId,
                        principalTable: "Crowdsource",
                        principalColumn: "UUID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Crowdsource_EntityId",
                table: "Crowdsource",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Crowdsource_SubmittedById",
                table: "Crowdsource",
                column: "SubmittedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrowdsourceVotes");

            migrationBuilder.DropTable(
                name: "Crowdsource");
        }
    }
}
