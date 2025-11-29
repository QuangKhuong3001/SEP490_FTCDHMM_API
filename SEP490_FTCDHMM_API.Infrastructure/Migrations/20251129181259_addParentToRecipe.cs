using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addParentToRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeUserTags_Recipes_RecipeId1",
                table: "RecipeUserTags");

            migrationBuilder.DropIndex(
                name: "IX_RecipeUserTags_RecipeId1",
                table: "RecipeUserTags");

            migrationBuilder.DropColumn(
                name: "RecipeId1",
                table: "RecipeUserTags");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Recipes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserLabelStats",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false),
                    SearchClicks = table.Column<int>(type: "int", nullable: false),
                    Favorites = table.Column<int>(type: "int", nullable: false),
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
                value: new DateTime(2025, 11, 29, 18, 12, 58, 376, DateTimeKind.Utc).AddTicks(8598));

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_ParentId",
                table: "Recipes",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLabelStats_LabelId",
                table: "UserLabelStats",
                column: "LabelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Recipes_ParentId",
                table: "Recipes",
                column: "ParentId",
                principalTable: "Recipes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Recipes_ParentId",
                table: "Recipes");

            migrationBuilder.DropTable(
                name: "UserLabelStats");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_ParentId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Recipes");

            migrationBuilder.AddColumn<Guid>(
                name: "RecipeId1",
                table: "RecipeUserTags",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 11, 26, 1, 47, 9, 549, DateTimeKind.Utc).AddTicks(5788));

            migrationBuilder.CreateIndex(
                name: "IX_RecipeUserTags_RecipeId1",
                table: "RecipeUserTags",
                column: "RecipeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeUserTags_Recipes_RecipeId1",
                table: "RecipeUserTags",
                column: "RecipeId1",
                principalTable: "Recipes",
                principalColumn: "Id");
        }
    }
}
