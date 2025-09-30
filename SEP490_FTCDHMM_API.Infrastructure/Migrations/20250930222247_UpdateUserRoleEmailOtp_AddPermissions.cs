using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserRoleEmailOtp_AddPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppRolePermission_AppRole_RolesId",
                table: "AppRolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_AppRolePermission_Permission_PermissionsId",
                table: "AppRolePermission");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.RenameColumn(
                name: "RolesId",
                table: "AppRolePermission",
                newName: "PermissionActionId");

            migrationBuilder.RenameColumn(
                name: "PermissionsId",
                table: "AppRolePermission",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AppRolePermission_RolesId",
                table: "AppRolePermission",
                newName: "IX_AppRolePermission_PermissionActionId");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AppRolePermission",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PermissionDomain",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionDomain", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionAction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PermissionDomainId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionAction_PermissionDomain_PermissionDomainId",
                        column: x => x.PermissionDomainId,
                        principalTable: "PermissionDomain",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PermissionDomain",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("58f211a8-1e64-c797-cb94-34ff7945f590"), "ModeratorManagement" },
                    { new Guid("9e0c2106-2ec3-4a03-2050-7e1aa77b3a3b"), "CustomerManagement" }
                });

            migrationBuilder.InsertData(
                table: "PermissionAction",
                columns: new[] { "Id", "Name", "PermissionDomainId" },
                values: new object[,]
                {
                    { new Guid("3c1f0712-eab0-cd34-b90d-62d1d886fd98"), "Update", new Guid("9e0c2106-2ec3-4a03-2050-7e1aa77b3a3b") },
                    { new Guid("5dbcfaf8-7006-f8be-cca0-e22622f58ea9"), "Delete", new Guid("9e0c2106-2ec3-4a03-2050-7e1aa77b3a3b") },
                    { new Guid("718dad89-4d50-185d-37e3-841f43d1a787"), "Update", new Guid("58f211a8-1e64-c797-cb94-34ff7945f590") },
                    { new Guid("bbe1aa5b-b75c-9427-9367-d86ca81437a6"), "Delete", new Guid("58f211a8-1e64-c797-cb94-34ff7945f590") },
                    { new Guid("ced5dfe6-7556-6848-28bc-774ca9373d65"), "Create", new Guid("58f211a8-1e64-c797-cb94-34ff7945f590") },
                    { new Guid("d6f9fd07-aa46-e05b-eb9a-3dbe142ff302"), "View", new Guid("58f211a8-1e64-c797-cb94-34ff7945f590") },
                    { new Guid("e5849211-12bb-edfc-75d8-de89ec0ec956"), "Create", new Guid("9e0c2106-2ec3-4a03-2050-7e1aa77b3a3b") },
                    { new Guid("e7b07d76-ce8e-5b94-f4e3-12de8c7a8382"), "View", new Guid("9e0c2106-2ec3-4a03-2050-7e1aa77b3a3b") }
                });

            migrationBuilder.InsertData(
                table: "AppRolePermission",
                columns: new[] { "PermissionActionId", "RoleId", "IsActive" },
                values: new object[,]
                {
                    { new Guid("3c1f0712-eab0-cd34-b90d-62d1d886fd98"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("5dbcfaf8-7006-f8be-cca0-e22622f58ea9"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("718dad89-4d50-185d-37e3-841f43d1a787"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("bbe1aa5b-b75c-9427-9367-d86ca81437a6"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("ced5dfe6-7556-6848-28bc-774ca9373d65"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("d6f9fd07-aa46-e05b-eb9a-3dbe142ff302"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("e5849211-12bb-edfc-75d8-de89ec0ec956"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("e7b07d76-ce8e-5b94-f4e3-12de8c7a8382"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true }
                });

            migrationBuilder.InsertData(
                table: "AppRolePermission",
                columns: new[] { "PermissionActionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("3c1f0712-eab0-cd34-b90d-62d1d886fd98"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("5dbcfaf8-7006-f8be-cca0-e22622f58ea9"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("718dad89-4d50-185d-37e3-841f43d1a787"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("bbe1aa5b-b75c-9427-9367-d86ca81437a6"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("ced5dfe6-7556-6848-28bc-774ca9373d65"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("d6f9fd07-aa46-e05b-eb9a-3dbe142ff302"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("e5849211-12bb-edfc-75d8-de89ec0ec956"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("e7b07d76-ce8e-5b94-f4e3-12de8c7a8382"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("3c1f0712-eab0-cd34-b90d-62d1d886fd98"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("5dbcfaf8-7006-f8be-cca0-e22622f58ea9"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("718dad89-4d50-185d-37e3-841f43d1a787"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("bbe1aa5b-b75c-9427-9367-d86ca81437a6"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("ced5dfe6-7556-6848-28bc-774ca9373d65"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("d6f9fd07-aa46-e05b-eb9a-3dbe142ff302"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("e5849211-12bb-edfc-75d8-de89ec0ec956"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("e7b07d76-ce8e-5b94-f4e3-12de8c7a8382"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionAction_PermissionDomainId",
                table: "PermissionAction",
                column: "PermissionDomainId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppRolePermission_AppRole_RoleId",
                table: "AppRolePermission",
                column: "RoleId",
                principalTable: "AppRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppRolePermission_PermissionAction_PermissionActionId",
                table: "AppRolePermission",
                column: "PermissionActionId",
                principalTable: "PermissionAction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppRolePermission_AppRole_RoleId",
                table: "AppRolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_AppRolePermission_PermissionAction_PermissionActionId",
                table: "AppRolePermission");

            migrationBuilder.DropTable(
                name: "PermissionAction");

            migrationBuilder.DropTable(
                name: "PermissionDomain");

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("3c1f0712-eab0-cd34-b90d-62d1d886fd98"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("5dbcfaf8-7006-f8be-cca0-e22622f58ea9"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("718dad89-4d50-185d-37e3-841f43d1a787"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("bbe1aa5b-b75c-9427-9367-d86ca81437a6"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("ced5dfe6-7556-6848-28bc-774ca9373d65"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("d6f9fd07-aa46-e05b-eb9a-3dbe142ff302"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("e5849211-12bb-edfc-75d8-de89ec0ec956"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("e7b07d76-ce8e-5b94-f4e3-12de8c7a8382"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("3c1f0712-eab0-cd34-b90d-62d1d886fd98"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("5dbcfaf8-7006-f8be-cca0-e22622f58ea9"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("718dad89-4d50-185d-37e3-841f43d1a787"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("bbe1aa5b-b75c-9427-9367-d86ca81437a6"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("ced5dfe6-7556-6848-28bc-774ca9373d65"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("d6f9fd07-aa46-e05b-eb9a-3dbe142ff302"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("e5849211-12bb-edfc-75d8-de89ec0ec956"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("e7b07d76-ce8e-5b94-f4e3-12de8c7a8382"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("3c1f0712-eab0-cd34-b90d-62d1d886fd98"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("5dbcfaf8-7006-f8be-cca0-e22622f58ea9"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("718dad89-4d50-185d-37e3-841f43d1a787"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("bbe1aa5b-b75c-9427-9367-d86ca81437a6"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("ced5dfe6-7556-6848-28bc-774ca9373d65"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("d6f9fd07-aa46-e05b-eb9a-3dbe142ff302"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("e5849211-12bb-edfc-75d8-de89ec0ec956"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("e7b07d76-ce8e-5b94-f4e3-12de8c7a8382"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AppRolePermission");

            migrationBuilder.RenameColumn(
                name: "PermissionActionId",
                table: "AppRolePermission",
                newName: "RolesId");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "AppRolePermission",
                newName: "PermissionsId");

            migrationBuilder.RenameIndex(
                name: "IX_AppRolePermission_PermissionActionId",
                table: "AppRolePermission",
                newName: "IX_AppRolePermission_RolesId");

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("58f211a8-1e64-c797-cb94-34ff7945f590"), "ModeratorManagement" },
                    { new Guid("9e0c2106-2ec3-4a03-2050-7e1aa77b3a3b"), "CustomerManagement" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AppRolePermission_AppRole_RolesId",
                table: "AppRolePermission",
                column: "RolesId",
                principalTable: "AppRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppRolePermission_Permission_PermissionsId",
                table: "AppRolePermission",
                column: "PermissionsId",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
