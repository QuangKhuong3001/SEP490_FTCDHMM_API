using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyEntityLabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LowerName",
                table: "Labels",
                newName: "UpperName");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 12, 21, 7, 38, 475, DateTimeKind.Utc).AddTicks(5773));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpperName",
                table: "Labels",
                newName: "LowerName");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 12, 19, 8, 16, 784, DateTimeKind.Utc).AddTicks(2740));
        }
    }
}
