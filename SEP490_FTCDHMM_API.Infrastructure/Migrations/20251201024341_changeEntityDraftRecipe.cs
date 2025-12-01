using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeEntityDraftRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DraftRecipes_Users_AuthorId",
                table: "DraftRecipes");

            migrationBuilder.DropIndex(
                name: "IX_DraftRecipes_AuthorId",
                table: "DraftRecipes");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 1, 2, 43, 41, 101, DateTimeKind.Utc).AddTicks(9976));

            migrationBuilder.CreateIndex(
                name: "IX_DraftRecipes_AuthorId",
                table: "DraftRecipes",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_DraftRecipes_Users_AuthorId",
                table: "DraftRecipes",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DraftRecipes_Users_AuthorId",
                table: "DraftRecipes");

            migrationBuilder.DropIndex(
                name: "IX_DraftRecipes_AuthorId",
                table: "DraftRecipes");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 11, 29, 18, 12, 58, 376, DateTimeKind.Utc).AddTicks(8598));

            migrationBuilder.CreateIndex(
                name: "IX_DraftRecipes_AuthorId",
                table: "DraftRecipes",
                column: "AuthorId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DraftRecipes_Users_AuthorId",
                table: "DraftRecipes",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
