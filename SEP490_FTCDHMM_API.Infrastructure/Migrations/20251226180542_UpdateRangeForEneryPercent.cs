using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRangeForEneryPercent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "EnergyPercent",
                table: "UserMealSlots",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,4)");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 26, 18, 5, 42, 267, DateTimeKind.Utc).AddTicks(553));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("133554ee-b8bf-0518-a055-4097baea7b64"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 18, 5, 42, 267, DateTimeKind.Utc).AddTicks(2772));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("16a7239f-04ef-4ae8-3c3d-f7c91f625ade"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 18, 5, 42, 267, DateTimeKind.Utc).AddTicks(2783));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("19f3c506-46ad-f9be-3a10-63dc2ed6a57e"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 18, 5, 42, 267, DateTimeKind.Utc).AddTicks(2794));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("8443f632-4d26-96c3-6c99-cdb180c761f3"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 18, 5, 42, 267, DateTimeKind.Utc).AddTicks(2789));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("b6cb3448-5f59-44b8-e69e-5a2e408ccd97"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 18, 5, 42, 267, DateTimeKind.Utc).AddTicks(2803));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("c8f90ed8-cc93-7d51-8477-534ff99d0fd0"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 18, 5, 42, 267, DateTimeKind.Utc).AddTicks(2809));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d238ef58-09be-5176-f430-16cdbfc0032a"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 18, 5, 42, 267, DateTimeKind.Utc).AddTicks(2814));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d5caeabc-0ca2-b778-f234-d5c084dd23cb"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 18, 5, 42, 267, DateTimeKind.Utc).AddTicks(2778));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("edae6e4e-e3a4-ccd4-a2d4-81edf652d3f4"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 18, 5, 42, 267, DateTimeKind.Utc).AddTicks(2760));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("f4a0ea3a-98b1-3443-4739-f63803a841c8"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 18, 5, 42, 267, DateTimeKind.Utc).AddTicks(2820));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 18, 5, 42, 267, DateTimeKind.Utc).AddTicks(923));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 18, 5, 42, 267, DateTimeKind.Utc).AddTicks(984));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 18, 5, 42, 267, DateTimeKind.Utc).AddTicks(973));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "EnergyPercent",
                table: "UserMealSlots",
                type: "decimal(5,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 26, 17, 59, 5, 944, DateTimeKind.Utc).AddTicks(7027));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("133554ee-b8bf-0518-a055-4097baea7b64"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 17, 59, 5, 944, DateTimeKind.Utc).AddTicks(9191));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("16a7239f-04ef-4ae8-3c3d-f7c91f625ade"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 17, 59, 5, 944, DateTimeKind.Utc).AddTicks(9202));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("19f3c506-46ad-f9be-3a10-63dc2ed6a57e"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 17, 59, 5, 944, DateTimeKind.Utc).AddTicks(9213));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("8443f632-4d26-96c3-6c99-cdb180c761f3"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 17, 59, 5, 944, DateTimeKind.Utc).AddTicks(9207));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("b6cb3448-5f59-44b8-e69e-5a2e408ccd97"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 17, 59, 5, 944, DateTimeKind.Utc).AddTicks(9218));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("c8f90ed8-cc93-7d51-8477-534ff99d0fd0"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 17, 59, 5, 944, DateTimeKind.Utc).AddTicks(9223));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d238ef58-09be-5176-f430-16cdbfc0032a"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 17, 59, 5, 944, DateTimeKind.Utc).AddTicks(9229));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d5caeabc-0ca2-b778-f234-d5c084dd23cb"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 17, 59, 5, 944, DateTimeKind.Utc).AddTicks(9197));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("edae6e4e-e3a4-ccd4-a2d4-81edf652d3f4"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 17, 59, 5, 944, DateTimeKind.Utc).AddTicks(9181));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("f4a0ea3a-98b1-3443-4739-f63803a841c8"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 17, 59, 5, 944, DateTimeKind.Utc).AddTicks(9234));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 17, 59, 5, 944, DateTimeKind.Utc).AddTicks(7383));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 17, 59, 5, 944, DateTimeKind.Utc).AddTicks(7431));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 17, 59, 5, 944, DateTimeKind.Utc).AddTicks(7423));
        }
    }
}
