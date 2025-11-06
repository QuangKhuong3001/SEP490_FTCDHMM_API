using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Comment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments");

            migrationBuilder.InsertData(
                table: "PermissionDomains",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("6940e80b-cd51-a8fd-2f00-f79328cf4efc"), "Comment" },
                    { new Guid("6fc0a9dd-0733-9b1c-6fc2-37ee164109d8"), "Rating" }
                });

            migrationBuilder.InsertData(
                table: "PermissionActions",
                columns: new[] { "Id", "Name", "PermissionDomainId" },
                values: new object[,]
                {
                    { new Guid("5afe8feb-08c8-d9b6-94d2-fb9c21306691"), "Update", new Guid("6940e80b-cd51-a8fd-2f00-f79328cf4efc") },
                    { new Guid("5d7a5bfd-19af-f795-4b79-0f893d244916"), "Create", new Guid("6fc0a9dd-0733-9b1c-6fc2-37ee164109d8") },
                    { new Guid("98fb23ef-e726-eab8-0fdc-9fc1d176a3d6"), "Delete", new Guid("6fc0a9dd-0733-9b1c-6fc2-37ee164109d8") },
                    { new Guid("c425362d-d120-a8cf-4afa-bca231428fc6"), "Create", new Guid("6940e80b-cd51-a8fd-2f00-f79328cf4efc") },
                    { new Guid("f7bc6e0d-5959-e608-01bd-08331d0a42a3"), "Delete", new Guid("6940e80b-cd51-a8fd-2f00-f79328cf4efc") },
                    { new Guid("fc41156f-a6e3-0cb2-f492-b5c324285a85"), "Update", new Guid("6fc0a9dd-0733-9b1c-6fc2-37ee164109d8") }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionActionId", "RoleId", "IsActive" },
                values: new object[,]
                {
                    { new Guid("5afe8feb-08c8-d9b6-94d2-fb9c21306691"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("5d7a5bfd-19af-f795-4b79-0f893d244916"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("98fb23ef-e726-eab8-0fdc-9fc1d176a3d6"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("c425362d-d120-a8cf-4afa-bca231428fc6"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("f7bc6e0d-5959-e608-01bd-08331d0a42a3"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("fc41156f-a6e3-0cb2-f492-b5c324285a85"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionActionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("5afe8feb-08c8-d9b6-94d2-fb9c21306691"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("5d7a5bfd-19af-f795-4b79-0f893d244916"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("98fb23ef-e726-eab8-0fdc-9fc1d176a3d6"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("c425362d-d120-a8cf-4afa-bca231428fc6"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("f7bc6e0d-5959-e608-01bd-08331d0a42a3"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("fc41156f-a6e3-0cb2-f492-b5c324285a85"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("5afe8feb-08c8-d9b6-94d2-fb9c21306691"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("5d7a5bfd-19af-f795-4b79-0f893d244916"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("98fb23ef-e726-eab8-0fdc-9fc1d176a3d6"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("c425362d-d120-a8cf-4afa-bca231428fc6"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("f7bc6e0d-5959-e608-01bd-08331d0a42a3"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("fc41156f-a6e3-0cb2-f492-b5c324285a85"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("5afe8feb-08c8-d9b6-94d2-fb9c21306691"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("5d7a5bfd-19af-f795-4b79-0f893d244916"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("98fb23ef-e726-eab8-0fdc-9fc1d176a3d6"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("c425362d-d120-a8cf-4afa-bca231428fc6"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("f7bc6e0d-5959-e608-01bd-08331d0a42a3"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("fc41156f-a6e3-0cb2-f492-b5c324285a85"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("5afe8feb-08c8-d9b6-94d2-fb9c21306691"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("5d7a5bfd-19af-f795-4b79-0f893d244916"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("98fb23ef-e726-eab8-0fdc-9fc1d176a3d6"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("c425362d-d120-a8cf-4afa-bca231428fc6"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("f7bc6e0d-5959-e608-01bd-08331d0a42a3"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("fc41156f-a6e3-0cb2-f492-b5c324285a85"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("5afe8feb-08c8-d9b6-94d2-fb9c21306691"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("5d7a5bfd-19af-f795-4b79-0f893d244916"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("98fb23ef-e726-eab8-0fdc-9fc1d176a3d6"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("c425362d-d120-a8cf-4afa-bca231428fc6"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("f7bc6e0d-5959-e608-01bd-08331d0a42a3"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("fc41156f-a6e3-0cb2-f492-b5c324285a85"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("5afe8feb-08c8-d9b6-94d2-fb9c21306691"));

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("5d7a5bfd-19af-f795-4b79-0f893d244916"));

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("98fb23ef-e726-eab8-0fdc-9fc1d176a3d6"));

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("c425362d-d120-a8cf-4afa-bca231428fc6"));

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("f7bc6e0d-5959-e608-01bd-08331d0a42a3"));

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("fc41156f-a6e3-0cb2-f492-b5c324285a85"));

            migrationBuilder.DeleteData(
                table: "PermissionDomains",
                keyColumn: "Id",
                keyValue: new Guid("6940e80b-cd51-a8fd-2f00-f79328cf4efc"));

            migrationBuilder.DeleteData(
                table: "PermissionDomains",
                keyColumn: "Id",
                keyValue: new Guid("6fc0a9dd-0733-9b1c-6fc2-37ee164109d8"));

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
