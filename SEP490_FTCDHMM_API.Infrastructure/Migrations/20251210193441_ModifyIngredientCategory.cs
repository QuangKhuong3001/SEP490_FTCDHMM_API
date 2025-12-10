using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyIngredientCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("7197f45f-8780-232a-b5a0-e2cde3ff35b7"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("af6fb429-d2e7-61f3-ab1d-400ac539aff8"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("7197f45f-8780-232a-b5a0-e2cde3ff35b7"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("af6fb429-d2e7-61f3-ab1d-400ac539aff8"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("7197f45f-8780-232a-b5a0-e2cde3ff35b7"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("af6fb429-d2e7-61f3-ab1d-400ac539aff8"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("7197f45f-8780-232a-b5a0-e2cde3ff35b7"));

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("af6fb429-d2e7-61f3-ab1d-400ac539aff8"));

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "IngredientCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpperName",
                table: "IngredientCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 10, 19, 34, 41, 633, DateTimeKind.Utc).AddTicks(4350));

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("0b391ac2-8440-b318-afc4-045c61aee15c"),
                columns: new[] { "NormalizedName", "UpperName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("0ef727db-5be6-f820-ec21-5d1d34876db5"),
                columns: new[] { "NormalizedName", "UpperName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("2510563b-4a1a-36f8-3eee-0081aeb1b15c"),
                columns: new[] { "NormalizedName", "UpperName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("36e6cb97-3dc3-c518-e22c-4d2804e5a65d"),
                columns: new[] { "NormalizedName", "UpperName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("3741c8e7-ce2b-c477-4e45-169cec441664"),
                columns: new[] { "NormalizedName", "UpperName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("447e8fa6-250f-0c6c-190e-d7ec87e91745"),
                columns: new[] { "NormalizedName", "UpperName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("5b8fd31b-bca6-bd0f-4bd4-1008a83f4385"),
                columns: new[] { "NormalizedName", "UpperName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("7814e36f-6b23-5d6b-f0b7-bc34f75fa315"),
                columns: new[] { "NormalizedName", "UpperName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("bcfbe809-1ee1-771d-e271-0f959bfd67f6"),
                columns: new[] { "NormalizedName", "UpperName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("db5072d7-9bc0-6d4a-8d33-3b18239c40f6"),
                columns: new[] { "NormalizedName", "UpperName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("e7f53468-c971-6d4d-7e56-1e50702495fd"),
                columns: new[] { "NormalizedName", "UpperName" },
                values: new object[] { "", "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "IngredientCategories");

            migrationBuilder.DropColumn(
                name: "UpperName",
                table: "IngredientCategories");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 9, 17, 20, 17, 522, DateTimeKind.Utc).AddTicks(4071));

            migrationBuilder.InsertData(
                table: "PermissionActions",
                columns: new[] { "Id", "Name", "PermissionDomainId" },
                values: new object[,]
                {
                    { new Guid("7197f45f-8780-232a-b5a0-e2cde3ff35b7"), "Cập nhật", new Guid("e32d29b4-2541-8830-3dde-355cf77e4a6d") },
                    { new Guid("af6fb429-d2e7-61f3-ab1d-400ac539aff8"), "Tạo", new Guid("e32d29b4-2541-8830-3dde-355cf77e4a6d") }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionActionId", "RoleId", "IsActive" },
                values: new object[,]
                {
                    { new Guid("7197f45f-8780-232a-b5a0-e2cde3ff35b7"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("af6fb429-d2e7-61f3-ab1d-400ac539aff8"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionActionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("7197f45f-8780-232a-b5a0-e2cde3ff35b7"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("af6fb429-d2e7-61f3-ab1d-400ac539aff8"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("7197f45f-8780-232a-b5a0-e2cde3ff35b7"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("af6fb429-d2e7-61f3-ab1d-400ac539aff8"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") }
                });
        }
    }
}
