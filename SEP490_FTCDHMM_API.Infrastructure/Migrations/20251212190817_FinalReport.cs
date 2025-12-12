using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FinalReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Users_UploadedById",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_UploadedById",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "UploadedById",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "LowerName",
                table: "Ingredients",
                newName: "UpperName");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 12, 19, 8, 16, 784, DateTimeKind.Utc).AddTicks(2740));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpperName",
                table: "Ingredients",
                newName: "LowerName");

            migrationBuilder.AddColumn<Guid>(
                name: "UploadedById",
                table: "Images",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                columns: new[] { "CreatedAtUTC", "UploadedById" },
                values: new object[] { new DateTime(2025, 12, 11, 18, 41, 1, 578, DateTimeKind.Utc).AddTicks(795), null });

            migrationBuilder.CreateIndex(
                name: "IX_Images_UploadedById",
                table: "Images",
                column: "UploadedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Users_UploadedById",
                table: "Images",
                column: "UploadedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
