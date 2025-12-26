using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMealSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserMealSlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EnergyPercent = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMealSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMealSlots_Users_UserId",
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

            migrationBuilder.CreateIndex(
                name: "IX_UserMealSlots_UserId_OrderIndex",
                table: "UserMealSlots",
                columns: new[] { "UserId", "OrderIndex" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMealSlots");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(4353));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("133554ee-b8bf-0518-a055-4097baea7b64"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6432));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("16a7239f-04ef-4ae8-3c3d-f7c91f625ade"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6471));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("19f3c506-46ad-f9be-3a10-63dc2ed6a57e"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6483));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("8443f632-4d26-96c3-6c99-cdb180c761f3"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6477));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("b6cb3448-5f59-44b8-e69e-5a2e408ccd97"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6488));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("c8f90ed8-cc93-7d51-8477-534ff99d0fd0"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6493));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d238ef58-09be-5176-f430-16cdbfc0032a"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6498));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d5caeabc-0ca2-b778-f234-d5c084dd23cb"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6438));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("edae6e4e-e3a4-ccd4-a2d4-81edf652d3f4"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6424));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("f4a0ea3a-98b1-3443-4739-f63803a841c8"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6504));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(4622));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(4673));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(4665));
        }
    }
}
