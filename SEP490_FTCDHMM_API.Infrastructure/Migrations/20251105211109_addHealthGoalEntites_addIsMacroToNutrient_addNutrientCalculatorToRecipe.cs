using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addHealthGoalEntites_addIsMacroToNutrient_addNutrientCalculatorToRecipe : Migration
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

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "Users");

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
                name: "Calories",
                table: "Recipes",
                type: "decimal(10,3)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "QuantityGram",
                table: "RecipeIngredients",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Nutrients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMacroNutrient",
                table: "Nutrients",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("3a9a556f-7285-4572-28aa-67447560ece8"),
                column: "Description",
                value: "Giúp tạo máu và duy trì hệ thần kinh.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("40d7e2f9-a5da-064c-fe4d-28febe860039"),
                column: "Description",
                value: "Chống oxy hóa, bảo vệ tế bào.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("422833be-5c43-e625-7a6a-6a74c32794a6"),
                column: "Description",
                value: "Chuyển hóa năng lượng, hỗ trợ thần kinh.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("4345a4c7-9cd2-6519-5892-9dcc40bb9ecc"),
                column: "Description",
                value: "Tăng cường miễn dịch, chống oxy hóa.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("4e465394-2d14-2a0a-7a00-5db0bc9e4597"),
                column: "Description",
                value: "Giúp tổng hợp hồng cầu, duy trì trao đổi chất.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("4e7a667e-4012-d80e-9276-1cd44d4e7fbd"),
                columns: new[] { "Description", "IsMacroNutrient" },
                values: new object[] { "Giúp xây dựng cơ bắp và tế bào.", true });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("55fb9dc4-6bf0-06b4-e2c5-ca786f557d38"),
                column: "Description",
                value: "Chống oxy hóa, tăng cường miễn dịch.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("5e08cf55-2b60-6f26-ef65-305553ffb09b"),
                column: "Description",
                value: "Giúp sáng mắt và tăng sức đề kháng.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("67dc4e3a-958b-9f2b-ba70-d7e6690b8f2d"),
                column: "Description",
                value: "Duy trì cân bằng nước và nhịp tim.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("73cd094d-61aa-61ce-d021-9ffa9b9ebbad"),
                columns: new[] { "Description", "IsMacroNutrient" },
                values: new object[] { "Tổng lượng chất béo trong thực phẩm.", true });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("7dd02ec7-0bde-e9d2-4f7b-99e3184f139e"),
                column: "Description",
                value: "Hỗ trợ miễn dịch, da, tóc và móng.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("7df9ddde-bcce-958a-2a38-85778c6cfb7b"),
                column: "Description",
                value: "Giúp đông máu và duy trì xương khỏe mạnh.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("88264feb-65c1-6808-c47c-44e3ebe1f725"),
                column: "Description",
                value: "Giúp hình thành xương và răng.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("968aface-8106-d49e-09dc-761ca6080887"),
                column: "Description",
                value: "Thành phần của huyết sắc tố (hemoglobin).");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("ac960903-ad9f-dfae-e0b3-35628565a3cb"),
                column: "Description",
                value: "Tốt cho da, mắt và hệ thần kinh.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("ba6906e3-9e16-e3df-06c5-f3b628919649"),
                column: "Description",
                value: "Giúp điều hòa nước và áp suất máu.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("bc5e858f-8aaa-e3f1-c7ae-bf691e5fa88e"),
                column: "Description",
                value: "Giúp chuyển hóa năng lượng, bảo vệ tim mạch.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c52f37b6-b8ba-c587-72d7-d3f5dc8044d6"),
                column: "Description",
                value: "Cần cho xương và não.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c8cd2a0b-6458-d98b-0ebf-0243cf575556"),
                column: "Description",
                value: "Giúp hấp thu canxi, tốt cho xương.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("dbe42ec4-51b1-f98d-66fe-6fed6bdcad0a"),
                column: "Description",
                value: "Tổng lượng đường tự nhiên và thêm vào.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("e19cd21d-2c38-e38f-7c55-f643fd65daf9"),
                column: "Description",
                value: "Hỗ trợ tiêu hóa và giảm cholesterol.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("ed0c64a9-afc7-216a-a83e-8aebc743e462"),
                column: "Description",
                value: "Cần thiết cho xương và răng chắc khỏe.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("f2e0b30a-40ad-f850-5251-36fd00dc462e"),
                column: "Description",
                value: "Cholesterol trong thực phẩm.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("f3c5dea5-8442-1e88-a8bb-d71679c86ede"),
                column: "Description",
                value: "Quan trọng cho cơ và thần kinh.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("fa0f09a4-fbbd-3da5-76b0-748a0d87ce21"),
                column: "Description",
                value: "Tham gia hình thành tế bào máu và enzyme.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("feca7dbc-1254-74f3-c7e0-ff7b786515d0"),
                columns: new[] { "Description", "IsMacroNutrient" },
                values: new object[] { "Nguồn năng lượng chính của cơ thể.", true });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("ff39565c-1b2f-7db1-4f47-7b9ca86221f6"),
                column: "Description",
                value: "Quan trọng cho phụ nữ mang thai và tế bào mới.");

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
                name: "Calories",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "QuantityGram",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Nutrients");

            migrationBuilder.DropColumn(
                name: "IsMacroNutrient",
                table: "Nutrients");

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

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
                columns: new[] { "Id", "Name", "UnitId", "VietnameseName" },
                values: new object[] { new Guid("d58dca3f-be87-c7d0-5396-223e9ced53a8"), "Calories", new Guid("93d2464a-59b3-7951-21fe-5dc36fe13c45"), "Năng lượng" });

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
