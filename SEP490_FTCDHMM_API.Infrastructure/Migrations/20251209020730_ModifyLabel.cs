using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyLabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LowerName",
                table: "Labels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Labels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 9, 2, 7, 30, 147, DateTimeKind.Utc).AddTicks(3380));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("133554ee-b8bf-0518-a055-4097baea7b64"),
                columns: new[] { "LowerName", "NormalizedName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("16a7239f-04ef-4ae8-3c3d-f7c91f625ade"),
                columns: new[] { "LowerName", "NormalizedName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("19f3c506-46ad-f9be-3a10-63dc2ed6a57e"),
                columns: new[] { "LowerName", "NormalizedName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("8443f632-4d26-96c3-6c99-cdb180c761f3"),
                columns: new[] { "LowerName", "NormalizedName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("b6cb3448-5f59-44b8-e69e-5a2e408ccd97"),
                columns: new[] { "LowerName", "NormalizedName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("c8f90ed8-cc93-7d51-8477-534ff99d0fd0"),
                columns: new[] { "LowerName", "NormalizedName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d238ef58-09be-5176-f430-16cdbfc0032a"),
                columns: new[] { "LowerName", "NormalizedName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d5caeabc-0ca2-b778-f234-d5c084dd23cb"),
                columns: new[] { "LowerName", "NormalizedName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("edae6e4e-e3a4-ccd4-a2d4-81edf652d3f4"),
                columns: new[] { "LowerName", "NormalizedName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("f4a0ea3a-98b1-3443-4739-f63803a841c8"),
                columns: new[] { "LowerName", "NormalizedName" },
                values: new object[] { "", "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LowerName",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Labels");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 9, 0, 24, 48, 292, DateTimeKind.Utc).AddTicks(4593));
        }
    }
}
