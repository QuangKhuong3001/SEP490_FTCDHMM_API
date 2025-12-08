using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 7, 16, 58, 40, 840, DateTimeKind.Utc).AddTicks(912));

            migrationBuilder.InsertData(
                table: "PermissionDomains",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("ca1dab66-9881-18a8-fd6d-8eb3d47f0422"), "Vai trò" });

            migrationBuilder.InsertData(
                table: "PermissionActions",
                columns: new[] { "Id", "Name", "PermissionDomainId" },
                values: new object[,]
                {
                    { new Guid("1af66f85-f3a2-4d60-3e56-462c7b6cfb83"), "Xóa", new Guid("ca1dab66-9881-18a8-fd6d-8eb3d47f0422") },
                    { new Guid("5b8783b4-85d6-c8de-a26a-6bc8c11f5aa9"), "Tạo", new Guid("ca1dab66-9881-18a8-fd6d-8eb3d47f0422") },
                    { new Guid("8375b114-7af3-ef43-fad8-a7e33dced94c"), "Xem", new Guid("ca1dab66-9881-18a8-fd6d-8eb3d47f0422") },
                    { new Guid("8e0e86e3-53f0-368e-c939-1a0e9604b738"), "Cập nhật", new Guid("ca1dab66-9881-18a8-fd6d-8eb3d47f0422") }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionActionId", "RoleId", "IsActive" },
                values: new object[,]
                {
                    { new Guid("1af66f85-f3a2-4d60-3e56-462c7b6cfb83"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("5b8783b4-85d6-c8de-a26a-6bc8c11f5aa9"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("8375b114-7af3-ef43-fad8-a7e33dced94c"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("8e0e86e3-53f0-368e-c939-1a0e9604b738"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionActionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("1af66f85-f3a2-4d60-3e56-462c7b6cfb83"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("5b8783b4-85d6-c8de-a26a-6bc8c11f5aa9"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("8375b114-7af3-ef43-fad8-a7e33dced94c"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("8e0e86e3-53f0-368e-c939-1a0e9604b738"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("1af66f85-f3a2-4d60-3e56-462c7b6cfb83"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("5b8783b4-85d6-c8de-a26a-6bc8c11f5aa9"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("8375b114-7af3-ef43-fad8-a7e33dced94c"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("8e0e86e3-53f0-368e-c939-1a0e9604b738"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("1af66f85-f3a2-4d60-3e56-462c7b6cfb83"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("5b8783b4-85d6-c8de-a26a-6bc8c11f5aa9"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("8375b114-7af3-ef43-fad8-a7e33dced94c"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("8e0e86e3-53f0-368e-c939-1a0e9604b738"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("1af66f85-f3a2-4d60-3e56-462c7b6cfb83"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("5b8783b4-85d6-c8de-a26a-6bc8c11f5aa9"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("8375b114-7af3-ef43-fad8-a7e33dced94c"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("8e0e86e3-53f0-368e-c939-1a0e9604b738"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("1af66f85-f3a2-4d60-3e56-462c7b6cfb83"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("5b8783b4-85d6-c8de-a26a-6bc8c11f5aa9"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("8375b114-7af3-ef43-fad8-a7e33dced94c"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("8e0e86e3-53f0-368e-c939-1a0e9604b738"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("1af66f85-f3a2-4d60-3e56-462c7b6cfb83"));

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("5b8783b4-85d6-c8de-a26a-6bc8c11f5aa9"));

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("8375b114-7af3-ef43-fad8-a7e33dced94c"));

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("8e0e86e3-53f0-368e-c939-1a0e9604b738"));

            migrationBuilder.DeleteData(
                table: "PermissionDomains",
                keyColumn: "Id",
                keyValue: new Guid("ca1dab66-9881-18a8-fd6d-8eb3d47f0422"));

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 7, 4, 12, 44, 434, DateTimeKind.Utc).AddTicks(9317));
        }
    }
}
