using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserHealthGoals",
                table: "UserHealthGoals");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "UserHealthGoals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserHealthGoals",
                table: "UserHealthGoals",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 1, 20, 15, 57, 59, DateTimeKind.Utc).AddTicks(8224));

            migrationBuilder.CreateIndex(
                name: "IX_UserHealthGoals_UserId",
                table: "UserHealthGoals",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserHealthGoals",
                table: "UserHealthGoals");

            migrationBuilder.DropIndex(
                name: "IX_UserHealthGoals_UserId",
                table: "UserHealthGoals");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserHealthGoals");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserHealthGoals",
                table: "UserHealthGoals",
                column: "UserId");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 1, 20, 13, 31, 954, DateTimeKind.Utc).AddTicks(5844));
        }
    }
}
