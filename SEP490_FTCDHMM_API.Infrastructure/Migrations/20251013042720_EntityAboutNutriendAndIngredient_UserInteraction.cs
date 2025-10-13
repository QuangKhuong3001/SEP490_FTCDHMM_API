using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EntityAboutNutriendAndIngredient_UserInteraction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UploadedById",
                table: "Image",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateTable(
                name: "IngredientCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UsageFrequency = table.Column<int>(type: "int", nullable: false),
                    SearchCount = table.Column<int>(type: "int", nullable: false),
                    PopularityScore = table.Column<double>(type: "float", nullable: false),
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingredients_Image_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Image",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NutrientUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutrientUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserFollows",
                columns: table => new
                {
                    FollowerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FolloweeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollows", x => new { x.FollowerId, x.FolloweeId });
                    table.ForeignKey(
                        name: "FK_UserFollows_AppUser_FolloweeId",
                        column: x => x.FolloweeId,
                        principalTable: "AppUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserFollows_AppUser_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "AppUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IngredientCategoryAssignments",
                columns: table => new
                {
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientCategoryAssignments", x => new { x.IngredientId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_IngredientCategoryAssignments_IngredientCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "IngredientCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientCategoryAssignments_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Nutrients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VietnameseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nutrients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nutrients_NutrientUnits_UnitId",
                        column: x => x.UnitId,
                        principalTable: "NutrientUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IngredientNutrients",
                columns: table => new
                {
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NutrientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Min = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: true),
                    Max = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: true),
                    Median = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientNutrients", x => new { x.IngredientId, x.NutrientId });
                    table.ForeignKey(
                        name: "FK_IngredientNutrients_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientNutrients_Nutrients_NutrientId",
                        column: x => x.NutrientId,
                        principalTable: "Nutrients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "IngredientCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("0b391ac2-8440-b318-afc4-045c61aee15c"), "Gia vị" },
                    { new Guid("0ef727db-5be6-f820-ec21-5d1d34876db5"), "Đồ uống" },
                    { new Guid("2510563b-4a1a-36f8-3eee-0081aeb1b15c"), "Đồ hộp / chế biến sẵn" },
                    { new Guid("36e6cb97-3dc3-c518-e22c-4d2804e5a65d"), "Hải sản" },
                    { new Guid("3741c8e7-ce2b-c477-4e45-169cec441664"), "Rau củ quả" },
                    { new Guid("447e8fa6-250f-0c6c-190e-d7ec87e91745"), "Nguyên liệu khác" },
                    { new Guid("5b8fd31b-bca6-bd0f-4bd4-1008a83f4385"), "Trứng" },
                    { new Guid("7814e36f-6b23-5d6b-f0b7-bc34f75fa315"), "Ngũ cốc" },
                    { new Guid("bcfbe809-1ee1-771d-e271-0f959bfd67f6"), "Đồ ngọt" },
                    { new Guid("db5072d7-9bc0-6d4a-8d33-3b18239c40f6"), "Dầu mỡ" },
                    { new Guid("e7f53468-c971-6d4d-7e56-1e50702495fd"), "Thịt" }
                });

            migrationBuilder.InsertData(
                table: "NutrientUnits",
                columns: new[] { "Id", "Description", "Name", "Symbol" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "Used for macronutrients like protein, fat, carbs", "Gram", "g" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Used for minerals and vitamins", "Milligram", "mg" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "Used for trace vitamins like B12", "Microgram", "µg" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "Unit of energy (Calories)", "Kilocalorie", "kcal" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "Alternative energy unit", "Kilojoule", "kJ" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "Used for liquid nutrients", "Milliliter", "mL" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "Used for large liquid volumes", "Liter", "L" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "Used for vitamin activity (A, D, E, K)", "International Unit", "IU" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "Percentage of daily value", "Percent", "%" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "No measurable unit", "None", "" },
                    { new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Đơn vị dùng cho các đại dưỡng chất như protein, chất béo, tinh bột.", "Gram (Gam)", "g" },
                    { new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Đơn vị dùng cho khoáng chất và vitamin thông thường.", "Milligram (Miligam)", "mg" },
                    { new Guid("93d2464a-59b3-7951-21fe-5dc36fe13c45"), "Đơn vị năng lượng thường gọi là calo.", "Kilocalorie (Kcal)", "kcal" },
                    { new Guid("a06cedbb-6209-6b82-bc1f-ca9873f9e31c"), "Dùng cho hoạt tính của vitamin A, D, E, K.", "International Unit (Đơn vị quốc tế)", "IU" },
                    { new Guid("c75b9051-0442-cf83-bf9d-c7c30b1413ad"), "Đơn vị dùng cho vitamin vi lượng như B12, K, Folate...", "Microgram (Micromet)", "µg" }
                });

            migrationBuilder.InsertData(
                table: "PermissionDomain",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("47831959-8aaa-9a40-d9e4-f0ccd56950eb"), "Ingredient" });

            migrationBuilder.InsertData(
                table: "Nutrients",
                columns: new[] { "Id", "Name", "UnitId", "VietnameseName" },
                values: new object[,]
                {
                    { new Guid("3a9a556f-7285-4572-28aa-67447560ece8"), "Vitamin B12", new Guid("c75b9051-0442-cf83-bf9d-c7c30b1413ad"), "Vitamin B12" },
                    { new Guid("40d7e2f9-a5da-064c-fe4d-28febe860039"), "Vitamin E", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Vitamin E" },
                    { new Guid("422833be-5c43-e625-7a6a-6a74c32794a6"), "Vitamin B1 (Thiamin)", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Vitamin B1" },
                    { new Guid("4345a4c7-9cd2-6519-5892-9dcc40bb9ecc"), "Vitamin C", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Vitamin C" },
                    { new Guid("4e465394-2d14-2a0a-7a00-5db0bc9e4597"), "Vitamin B6", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Vitamin B6" },
                    { new Guid("4e7a667e-4012-d80e-9276-1cd44d4e7fbd"), "Protein", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Chất đạm" },
                    { new Guid("55fb9dc4-6bf0-06b4-e2c5-ca786f557d38"), "Selenium", new Guid("c75b9051-0442-cf83-bf9d-c7c30b1413ad"), "Selen" },
                    { new Guid("5e08cf55-2b60-6f26-ef65-305553ffb09b"), "Vitamin A", new Guid("a06cedbb-6209-6b82-bc1f-ca9873f9e31c"), "Vitamin A" },
                    { new Guid("67dc4e3a-958b-9f2b-ba70-d7e6690b8f2d"), "Potassium", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Kali" },
                    { new Guid("73cd094d-61aa-61ce-d021-9ffa9b9ebbad"), "Fat", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Tổng chất béo" },
                    { new Guid("7dd02ec7-0bde-e9d2-4f7b-99e3184f139e"), "Zinc", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Kẽm" },
                    { new Guid("7df9ddde-bcce-958a-2a38-85778c6cfb7b"), "Vitamin K", new Guid("c75b9051-0442-cf83-bf9d-c7c30b1413ad"), "Vitamin K" },
                    { new Guid("88264feb-65c1-6808-c47c-44e3ebe1f725"), "Phosphorus", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Phốt pho" },
                    { new Guid("968aface-8106-d49e-09dc-761ca6080887"), "Iron", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Sắt" },
                    { new Guid("ac960903-ad9f-dfae-e0b3-35628565a3cb"), "Vitamin B2 (Riboflavin)", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Vitamin B2" },
                    { new Guid("ba6906e3-9e16-e3df-06c5-f3b628919649"), "Sodium", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Natri" },
                    { new Guid("bc5e858f-8aaa-e3f1-c7ae-bf691e5fa88e"), "Vitamin B3 (Niacin)", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Vitamin B3" },
                    { new Guid("c52f37b6-b8ba-c587-72d7-d3f5dc8044d6"), "Manganese", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Mangan" },
                    { new Guid("c8cd2a0b-6458-d98b-0ebf-0243cf575556"), "Vitamin D", new Guid("a06cedbb-6209-6b82-bc1f-ca9873f9e31c"), "Vitamin D" },
                    { new Guid("d58dca3f-be87-c7d0-5396-223e9ced53a8"), "Calories", new Guid("93d2464a-59b3-7951-21fe-5dc36fe13c45"), "Năng lượng" },
                    { new Guid("dbe42ec4-51b1-f98d-66fe-6fed6bdcad0a"), "Sugars", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Đường" },
                    { new Guid("e19cd21d-2c38-e38f-7c55-f643fd65daf9"), "Dietary Fiber", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Chất xơ" },
                    { new Guid("ed0c64a9-afc7-216a-a83e-8aebc743e462"), "Calcium", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Canxi" },
                    { new Guid("f2e0b30a-40ad-f850-5251-36fd00dc462e"), "Cholesterol", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Cholesterol" },
                    { new Guid("f3c5dea5-8442-1e88-a8bb-d71679c86ede"), "Magnesium", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Magie" },
                    { new Guid("fa0f09a4-fbbd-3da5-76b0-748a0d87ce21"), "Copper", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Đồng" },
                    { new Guid("feca7dbc-1254-74f3-c7e0-ff7b786515d0"), "Carbohydrate", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Tinh bột" },
                    { new Guid("ff39565c-1b2f-7db1-4f47-7b9ca86221f6"), "Folate (Folic Acid)", new Guid("c75b9051-0442-cf83-bf9d-c7c30b1413ad"), "Axit folic" }
                });

            migrationBuilder.InsertData(
                table: "PermissionAction",
                columns: new[] { "Id", "Name", "PermissionDomainId" },
                values: new object[,]
                {
                    { new Guid("394d428f-622a-87d8-fb05-9267ceb6a15c"), "Update", new Guid("47831959-8aaa-9a40-d9e4-f0ccd56950eb") },
                    { new Guid("6d22c125-a619-3aac-7483-3c351375f99a"), "Create", new Guid("47831959-8aaa-9a40-d9e4-f0ccd56950eb") },
                    { new Guid("f737e8f4-b9d0-8044-ec57-6d51a183a4cc"), "Delete", new Guid("47831959-8aaa-9a40-d9e4-f0ccd56950eb") }
                });

            migrationBuilder.InsertData(
                table: "AppRolePermission",
                columns: new[] { "PermissionActionId", "RoleId", "IsActive" },
                values: new object[,]
                {
                    { new Guid("394d428f-622a-87d8-fb05-9267ceb6a15c"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("6d22c125-a619-3aac-7483-3c351375f99a"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("f737e8f4-b9d0-8044-ec57-6d51a183a4cc"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true }
                });

            migrationBuilder.InsertData(
                table: "AppRolePermission",
                columns: new[] { "PermissionActionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("394d428f-622a-87d8-fb05-9267ceb6a15c"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("6d22c125-a619-3aac-7483-3c351375f99a"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("f737e8f4-b9d0-8044-ec57-6d51a183a4cc"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("394d428f-622a-87d8-fb05-9267ceb6a15c"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("6d22c125-a619-3aac-7483-3c351375f99a"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("f737e8f4-b9d0-8044-ec57-6d51a183a4cc"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategories_Name",
                table: "IngredientCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategoryAssignments_CategoryId",
                table: "IngredientCategoryAssignments",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientNutrients_IngredientId",
                table: "IngredientNutrients",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientNutrients_NutrientId",
                table: "IngredientNutrients",
                column: "NutrientId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_ImageId",
                table: "Ingredients",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_Name",
                table: "Ingredients",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nutrients_UnitId",
                table: "Nutrients",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollows_FolloweeId",
                table: "UserFollows",
                column: "FolloweeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientCategoryAssignments");

            migrationBuilder.DropTable(
                name: "IngredientNutrients");

            migrationBuilder.DropTable(
                name: "UserFollows");

            migrationBuilder.DropTable(
                name: "IngredientCategories");

            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropTable(
                name: "Nutrients");

            migrationBuilder.DropTable(
                name: "NutrientUnits");

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("394d428f-622a-87d8-fb05-9267ceb6a15c"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("6d22c125-a619-3aac-7483-3c351375f99a"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("f737e8f4-b9d0-8044-ec57-6d51a183a4cc"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("394d428f-622a-87d8-fb05-9267ceb6a15c"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("6d22c125-a619-3aac-7483-3c351375f99a"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("f737e8f4-b9d0-8044-ec57-6d51a183a4cc"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("394d428f-622a-87d8-fb05-9267ceb6a15c"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("6d22c125-a619-3aac-7483-3c351375f99a"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "AppRolePermission",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("f737e8f4-b9d0-8044-ec57-6d51a183a4cc"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "PermissionAction",
                keyColumn: "Id",
                keyValue: new Guid("394d428f-622a-87d8-fb05-9267ceb6a15c"));

            migrationBuilder.DeleteData(
                table: "PermissionAction",
                keyColumn: "Id",
                keyValue: new Guid("6d22c125-a619-3aac-7483-3c351375f99a"));

            migrationBuilder.DeleteData(
                table: "PermissionAction",
                keyColumn: "Id",
                keyValue: new Guid("f737e8f4-b9d0-8044-ec57-6d51a183a4cc"));

            migrationBuilder.DeleteData(
                table: "PermissionDomain",
                keyColumn: "Id",
                keyValue: new Guid("47831959-8aaa-9a40-d9e4-f0ccd56950eb"));

            migrationBuilder.AlterColumn<Guid>(
                name: "UploadedById",
                table: "Image",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
