using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Roport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReporterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ReviewedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RejectReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_Users_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 11, 30, 14, 24, 43, 263, DateTimeKind.Utc).AddTicks(8330));

            migrationBuilder.InsertData(
                table: "PermissionDomains",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("c84d1b4b-38cf-c6b3-4b1d-657da8f5ac8c"), "Report" });

            migrationBuilder.InsertData(
                table: "PermissionActions",
                columns: new[] { "Id", "Name", "PermissionDomainId" },
                values: new object[,]
                {
                    { new Guid("11cf4815-8318-cb00-ce7c-92b61942ea34"), "View", new Guid("c84d1b4b-38cf-c6b3-4b1d-657da8f5ac8c") },
                    { new Guid("311308d9-db5f-318c-7d23-bf56668c977f"), "Approve", new Guid("c84d1b4b-38cf-c6b3-4b1d-657da8f5ac8c") },
                    { new Guid("4513ef58-7b84-9d14-33ff-4af1f4de7bb7"), "Reject", new Guid("c84d1b4b-38cf-c6b3-4b1d-657da8f5ac8c") }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionActionId", "RoleId", "IsActive" },
                values: new object[,]
                {
                    { new Guid("11cf4815-8318-cb00-ce7c-92b61942ea34"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("311308d9-db5f-318c-7d23-bf56668c977f"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("4513ef58-7b84-9d14-33ff-4af1f4de7bb7"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionActionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("11cf4815-8318-cb00-ce7c-92b61942ea34"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("311308d9-db5f-318c-7d23-bf56668c977f"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("4513ef58-7b84-9d14-33ff-4af1f4de7bb7"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("11cf4815-8318-cb00-ce7c-92b61942ea34"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("311308d9-db5f-318c-7d23-bf56668c977f"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("4513ef58-7b84-9d14-33ff-4af1f4de7bb7"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReporterId",
                table: "Reports",
                column: "ReporterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("11cf4815-8318-cb00-ce7c-92b61942ea34"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("311308d9-db5f-318c-7d23-bf56668c977f"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("4513ef58-7b84-9d14-33ff-4af1f4de7bb7"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("11cf4815-8318-cb00-ce7c-92b61942ea34"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("311308d9-db5f-318c-7d23-bf56668c977f"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("4513ef58-7b84-9d14-33ff-4af1f4de7bb7"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("11cf4815-8318-cb00-ce7c-92b61942ea34"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("311308d9-db5f-318c-7d23-bf56668c977f"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("4513ef58-7b84-9d14-33ff-4af1f4de7bb7"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("11cf4815-8318-cb00-ce7c-92b61942ea34"));

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("311308d9-db5f-318c-7d23-bf56668c977f"));

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("4513ef58-7b84-9d14-33ff-4af1f4de7bb7"));

            migrationBuilder.DeleteData(
                table: "PermissionDomains",
                keyColumn: "Id",
                keyValue: new Guid("c84d1b4b-38cf-c6b3-4b1d-657da8f5ac8c"));

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 11, 26, 1, 47, 9, 549, DateTimeKind.Utc).AddTicks(5788));
        }
    }
}
