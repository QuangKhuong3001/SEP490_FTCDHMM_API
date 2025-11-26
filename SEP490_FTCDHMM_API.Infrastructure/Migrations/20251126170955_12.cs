using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLabelStats",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false),
                    SearchClicks = table.Column<int>(type: "int", nullable: false),
                    Likes = table.Column<int>(type: "int", nullable: false),
                    Saves = table.Column<int>(type: "int", nullable: false),
                    Ratings = table.Column<int>(type: "int", nullable: false),
                    RatingSum = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLabelStats", x => new { x.UserId, x.LabelId });
                    table.ForeignKey(
                        name: "FK_UserLabelStats_Labels_LabelId",
                        column: x => x.LabelId,
                        principalTable: "Labels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLabelStats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 11, 26, 17, 9, 55, 367, DateTimeKind.Utc).AddTicks(5893));

            migrationBuilder.CreateIndex(
                name: "IX_UserLabelStats_LabelId",
                table: "UserLabelStats",
                column: "LabelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLabelStats");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 11, 26, 1, 47, 9, 549, DateTimeKind.Utc).AddTicks(5788));
        }
    }
}
