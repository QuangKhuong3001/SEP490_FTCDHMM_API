using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fasf : Migration
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

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 11, 28, 23, 17, 14, 14, DateTimeKind.Utc).AddTicks(2162));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                value: new DateTime(2025, 11, 28, 23, 12, 38, 845, DateTimeKind.Utc).AddTicks(405));

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
