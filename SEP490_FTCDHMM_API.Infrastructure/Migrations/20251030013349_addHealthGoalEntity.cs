using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addHealthGoalEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_Ingredients_IngredientsId",
                table: "RecipeIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_Recipes_RecipesId",
                table: "RecipeIngredients");

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("d58dca3f-be87-c7d0-5396-223e9ced53a8"));

            migrationBuilder.RenameColumn(
                name: "RecipesId",
                table: "RecipeIngredients",
                newName: "IngredientId");

            migrationBuilder.RenameColumn(
                name: "IngredientsId",
                table: "RecipeIngredients",
                newName: "RecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_RecipeIngredients_RecipesId",
                table: "RecipeIngredients",
                newName: "IX_RecipeIngredients_IngredientId");

            migrationBuilder.RenameColumn(
                name: "Min",
                table: "IngredientNutrients",
                newName: "MinValue");

            migrationBuilder.RenameColumn(
                name: "Median",
                table: "IngredientNutrients",
                newName: "MedianValue");

            migrationBuilder.RenameColumn(
                name: "Max",
                table: "IngredientNutrients",
                newName: "MaxValue");

            migrationBuilder.AlterColumn<int>(
                name: "CookTime",
                table: "Recipes",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(10)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AddColumn<decimal>(
                name: "QuantityGram",
                table: "RecipeIngredients",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Calories",
                table: "Ingredients",
                type: "decimal(10,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "CustomHealthGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomHealthGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomHealthGoals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HealthGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthGoals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecipeNutritionAggregates",
                columns: table => new
                {
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NutrientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    AmountPerServing = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ComputedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeNutritionAggregates", x => new { x.RecipeId, x.NutrientId });
                    table.ForeignKey(
                        name: "FK_RecipeNutritionAggregates_Nutrients_NutrientId",
                        column: x => x.NutrientId,
                        principalTable: "Nutrients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeNutritionAggregates_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomHealthGoalTargets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomHealthGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NutrientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    MinEnergyPct = table.Column<decimal>(type: "decimal(6,4)", precision: 6, scale: 4, nullable: true),
                    MaxEnergyPct = table.Column<decimal>(type: "decimal(6,4)", precision: 6, scale: 4, nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(9,3)", precision: 9, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomHealthGoalTargets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomHealthGoalTargets_CustomHealthGoals_CustomHealthGoalId",
                        column: x => x.CustomHealthGoalId,
                        principalTable: "CustomHealthGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomHealthGoalTargets_Nutrients_NutrientId",
                        column: x => x.NutrientId,
                        principalTable: "Nutrients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HealthGoalConflicts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HealthGoalAId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HealthGoalBId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthGoalConflicts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthGoalConflicts_HealthGoals_HealthGoalAId",
                        column: x => x.HealthGoalAId,
                        principalTable: "HealthGoals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HealthGoalConflicts_HealthGoals_HealthGoalBId",
                        column: x => x.HealthGoalBId,
                        principalTable: "HealthGoals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HealthGoalTargets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HealthGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NutrientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    MinEnergyPct = table.Column<decimal>(type: "decimal(6,4)", precision: 6, scale: 4, nullable: true),
                    MaxEnergyPct = table.Column<decimal>(type: "decimal(6,4)", precision: 6, scale: 4, nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(9,3)", precision: 9, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthGoalTargets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthGoalTargets_HealthGoals_HealthGoalId",
                        column: x => x.HealthGoalId,
                        principalTable: "HealthGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HealthGoalTargets_Nutrients_NutrientId",
                        column: x => x.NutrientId,
                        principalTable: "Nutrients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserHealthGoals",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HealthGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHealthGoals", x => new { x.UserId, x.HealthGoalId });
                    table.ForeignKey(
                        name: "FK_UserHealthGoals_HealthGoals_HealthGoalId",
                        column: x => x.HealthGoalId,
                        principalTable: "HealthGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserHealthGoals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PermissionDomains",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("7f3cc217-2b00-adff-c855-c738a34c7183"), "HealthGoal" });

            migrationBuilder.InsertData(
                table: "PermissionActions",
                columns: new[] { "Id", "Name", "PermissionDomainId" },
                values: new object[,]
                {
                    { new Guid("5e446033-c846-8d05-e416-f9ceb3e3d829"), "Create", new Guid("7f3cc217-2b00-adff-c855-c738a34c7183") },
                    { new Guid("9c8bbbf1-22d6-bfd3-2c0b-6ec22bdc29c6"), "Delete", new Guid("7f3cc217-2b00-adff-c855-c738a34c7183") },
                    { new Guid("bba0d6e7-3d61-14c2-5658-0316d1679c01"), "Update", new Guid("7f3cc217-2b00-adff-c855-c738a34c7183") }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionActionId", "RoleId", "IsActive" },
                values: new object[,]
                {
                    { new Guid("5e446033-c846-8d05-e416-f9ceb3e3d829"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("9c8bbbf1-22d6-bfd3-2c0b-6ec22bdc29c6"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("bba0d6e7-3d61-14c2-5658-0316d1679c01"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionActionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("5e446033-c846-8d05-e416-f9ceb3e3d829"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("9c8bbbf1-22d6-bfd3-2c0b-6ec22bdc29c6"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("bba0d6e7-3d61-14c2-5658-0316d1679c01"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("5e446033-c846-8d05-e416-f9ceb3e3d829"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("9c8bbbf1-22d6-bfd3-2c0b-6ec22bdc29c6"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("bba0d6e7-3d61-14c2-5658-0316d1679c01"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomHealthGoals_UserId",
                table: "CustomHealthGoals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomHealthGoalTargets_CustomHealthGoalId",
                table: "CustomHealthGoalTargets",
                column: "CustomHealthGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomHealthGoalTargets_NutrientId",
                table: "CustomHealthGoalTargets",
                column: "NutrientId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthGoalConflicts_HealthGoalAId_HealthGoalBId",
                table: "HealthGoalConflicts",
                columns: new[] { "HealthGoalAId", "HealthGoalBId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HealthGoalConflicts_HealthGoalBId",
                table: "HealthGoalConflicts",
                column: "HealthGoalBId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthGoalTargets_HealthGoalId",
                table: "HealthGoalTargets",
                column: "HealthGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthGoalTargets_NutrientId",
                table: "HealthGoalTargets",
                column: "NutrientId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeNutritionAggregates_NutrientId",
                table: "RecipeNutritionAggregates",
                column: "NutrientId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHealthGoals_HealthGoalId",
                table: "UserHealthGoals",
                column: "HealthGoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_Ingredients_IngredientId",
                table: "RecipeIngredients",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_Recipes_RecipeId",
                table: "RecipeIngredients",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_Ingredients_IngredientId",
                table: "RecipeIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_Recipes_RecipeId",
                table: "RecipeIngredients");

            migrationBuilder.DropTable(
                name: "CustomHealthGoalTargets");

            migrationBuilder.DropTable(
                name: "HealthGoalConflicts");

            migrationBuilder.DropTable(
                name: "HealthGoalTargets");

            migrationBuilder.DropTable(
                name: "RecipeNutritionAggregates");

            migrationBuilder.DropTable(
                name: "UserHealthGoals");

            migrationBuilder.DropTable(
                name: "CustomHealthGoals");

            migrationBuilder.DropTable(
                name: "HealthGoals");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("5e446033-c846-8d05-e416-f9ceb3e3d829"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("9c8bbbf1-22d6-bfd3-2c0b-6ec22bdc29c6"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("bba0d6e7-3d61-14c2-5658-0316d1679c01"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("5e446033-c846-8d05-e416-f9ceb3e3d829"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("9c8bbbf1-22d6-bfd3-2c0b-6ec22bdc29c6"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("bba0d6e7-3d61-14c2-5658-0316d1679c01"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("5e446033-c846-8d05-e416-f9ceb3e3d829"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("9c8bbbf1-22d6-bfd3-2c0b-6ec22bdc29c6"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("bba0d6e7-3d61-14c2-5658-0316d1679c01"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("5e446033-c846-8d05-e416-f9ceb3e3d829"));

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("9c8bbbf1-22d6-bfd3-2c0b-6ec22bdc29c6"));

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("bba0d6e7-3d61-14c2-5658-0316d1679c01"));

            migrationBuilder.DeleteData(
                table: "PermissionDomains",
                keyColumn: "Id",
                keyValue: new Guid("7f3cc217-2b00-adff-c855-c738a34c7183"));

            migrationBuilder.DropColumn(
                name: "QuantityGram",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "Calories",
                table: "Ingredients");

            migrationBuilder.RenameColumn(
                name: "IngredientId",
                table: "RecipeIngredients",
                newName: "RecipesId");

            migrationBuilder.RenameColumn(
                name: "RecipeId",
                table: "RecipeIngredients",
                newName: "IngredientsId");

            migrationBuilder.RenameIndex(
                name: "IX_RecipeIngredients_IngredientId",
                table: "RecipeIngredients",
                newName: "IX_RecipeIngredients_RecipesId");

            migrationBuilder.RenameColumn(
                name: "MinValue",
                table: "IngredientNutrients",
                newName: "Min");

            migrationBuilder.RenameColumn(
                name: "MedianValue",
                table: "IngredientNutrients",
                newName: "Median");

            migrationBuilder.RenameColumn(
                name: "MaxValue",
                table: "IngredientNutrients",
                newName: "Max");

            migrationBuilder.AlterColumn<double>(
                name: "CookTime",
                table: "Recipes",
                type: "float(10)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "Nutrients",
                columns: new[] { "Id", "Description", "IsRequired", "Name", "UnitId", "VietnameseName" },
                values: new object[] { new Guid("d58dca3f-be87-c7d0-5396-223e9ced53a8"), "Tổng năng lượng cung cấp (Energy)", true, "Calories", new Guid("93d2464a-59b3-7951-21fe-5dc36fe13c45"), "Năng lượng" });

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_Ingredients_IngredientsId",
                table: "RecipeIngredients",
                column: "IngredientsId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_Recipes_RecipesId",
                table: "RecipeIngredients",
                column: "RecipesId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
