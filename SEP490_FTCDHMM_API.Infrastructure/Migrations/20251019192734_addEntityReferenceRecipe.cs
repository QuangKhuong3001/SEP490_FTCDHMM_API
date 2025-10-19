using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addEntityReferenceRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionActions_PermissionDomain_PermissionDomainId",
                table: "PermissionActions");

            migrationBuilder.DropTable(
                name: "IngredientCategoryAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PermissionDomain",
                table: "PermissionDomain");

            migrationBuilder.DeleteData(
                table: "NutrientUnits",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "NutrientUnits",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "NutrientUnits",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "NutrientUnits",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "NutrientUnits",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "NutrientUnits",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "NutrientUnits",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "NutrientUnits",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "NutrientUnits",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "NutrientUnits",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"));

            migrationBuilder.DropColumn(
                name: "Instruction",
                table: "Recipes");

            migrationBuilder.RenameTable(
                name: "PermissionDomain",
                newName: "PermissionDomains");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Recipes",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageId",
                table: "Recipes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Ration",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "Recipes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ColorCode",
                table: "Labels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "Labels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "IngredientCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PermissionDomains",
                table: "PermissionDomains",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CookingSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Instruction = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CookingSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CookingSteps_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CookingSteps_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IngredientCategoryLink",
                columns: table => new
                {
                    CategoriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredientsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientCategoryLink", x => new { x.CategoriesId, x.IngredientsId });
                    table.ForeignKey(
                        name: "FK_IngredientCategoryLink_IngredientCategories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "IngredientCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientCategoryLink_Ingredients_IngredientsId",
                        column: x => x.IngredientsId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavoriteRecipes",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteRecipes", x => new { x.UserId, x.RecipeId });
                    table.ForeignKey(
                        name: "FK_UserFavoriteRecipes_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavoriteRecipes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRecipeViews",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRecipeViews", x => new { x.UserId, x.RecipeId });
                    table.ForeignKey(
                        name: "FK_UserRecipeViews_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRecipeViews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSaveRecipes",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSaveRecipes", x => new { x.UserId, x.RecipeId });
                    table.ForeignKey(
                        name: "FK_UserSaveRecipes_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSaveRecipes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("0b391ac2-8440-b318-afc4-045c61aee15c"),
                column: "isDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("0ef727db-5be6-f820-ec21-5d1d34876db5"),
                column: "isDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("2510563b-4a1a-36f8-3eee-0081aeb1b15c"),
                column: "isDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("36e6cb97-3dc3-c518-e22c-4d2804e5a65d"),
                column: "isDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("3741c8e7-ce2b-c477-4e45-169cec441664"),
                column: "isDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("447e8fa6-250f-0c6c-190e-d7ec87e91745"),
                column: "isDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("5b8fd31b-bca6-bd0f-4bd4-1008a83f4385"),
                column: "isDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("7814e36f-6b23-5d6b-f0b7-bc34f75fa315"),
                column: "isDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("bcfbe809-1ee1-771d-e271-0f959bfd67f6"),
                column: "isDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("db5072d7-9bc0-6d4a-8d33-3b18239c40f6"),
                column: "isDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "IngredientCategories",
                keyColumn: "Id",
                keyValue: new Guid("e7f53468-c971-6d4d-7e56-1e50702495fd"),
                column: "isDeleted",
                value: false);

            migrationBuilder.InsertData(
                table: "PermissionDomains",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("6adf21b0-46ac-c454-54f4-6c77646e745f"), "Label" },
                    { new Guid("f90072cc-a782-723a-e251-e25bc6ca5e6f"), "IngredientCategory" }
                });

            migrationBuilder.InsertData(
                table: "PermissionActions",
                columns: new[] { "Id", "Name", "PermissionDomainId" },
                values: new object[,]
                {
                    { new Guid("104dfcfa-1ea8-e98c-86d6-2a54dfc76667"), "Create", new Guid("6adf21b0-46ac-c454-54f4-6c77646e745f") },
                    { new Guid("4946df8e-30a3-6ab7-5f45-bef28f0536bc"), "Update", new Guid("6adf21b0-46ac-c454-54f4-6c77646e745f") },
                    { new Guid("4aaf7650-f5b9-6640-a4da-f851a49e6d16"), "Delete", new Guid("6adf21b0-46ac-c454-54f4-6c77646e745f") },
                    { new Guid("7bd0c333-0cc5-a866-0902-8d606e59e9de"), "Delete", new Guid("f90072cc-a782-723a-e251-e25bc6ca5e6f") },
                    { new Guid("b9e09fc1-bafc-f5aa-2396-91ca05ac7839"), "Create", new Guid("f90072cc-a782-723a-e251-e25bc6ca5e6f") }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionActionId", "RoleId", "IsActive" },
                values: new object[,]
                {
                    { new Guid("104dfcfa-1ea8-e98c-86d6-2a54dfc76667"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("4946df8e-30a3-6ab7-5f45-bef28f0536bc"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("4aaf7650-f5b9-6640-a4da-f851a49e6d16"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("7bd0c333-0cc5-a866-0902-8d606e59e9de"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true },
                    { new Guid("b9e09fc1-bafc-f5aa-2396-91ca05ac7839"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"), true }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionActionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("104dfcfa-1ea8-e98c-86d6-2a54dfc76667"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("4946df8e-30a3-6ab7-5f45-bef28f0536bc"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("4aaf7650-f5b9-6640-a4da-f851a49e6d16"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("7bd0c333-0cc5-a866-0902-8d606e59e9de"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("b9e09fc1-bafc-f5aa-2396-91ca05ac7839"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") },
                    { new Guid("104dfcfa-1ea8-e98c-86d6-2a54dfc76667"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("4946df8e-30a3-6ab7-5f45-bef28f0536bc"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("4aaf7650-f5b9-6640-a4da-f851a49e6d16"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("7bd0c333-0cc5-a866-0902-8d606e59e9de"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") },
                    { new Guid("b9e09fc1-bafc-f5aa-2396-91ca05ac7839"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_ImageId",
                table: "Recipes",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CookingSteps_ImageId",
                table: "CookingSteps",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_CookingSteps_RecipeId",
                table: "CookingSteps",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategoryLink_IngredientsId",
                table: "IngredientCategoryLink",
                column: "IngredientsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteRecipes_RecipeId",
                table: "UserFavoriteRecipes",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRecipeViews_RecipeId",
                table: "UserRecipeViews",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSaveRecipes_RecipeId",
                table: "UserSaveRecipes",
                column: "RecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionActions_PermissionDomains_PermissionDomainId",
                table: "PermissionActions",
                column: "PermissionDomainId",
                principalTable: "PermissionDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Images_ImageId",
                table: "Recipes",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionActions_PermissionDomains_PermissionDomainId",
                table: "PermissionActions");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Images_ImageId",
                table: "Recipes");

            migrationBuilder.DropTable(
                name: "CookingSteps");

            migrationBuilder.DropTable(
                name: "IngredientCategoryLink");

            migrationBuilder.DropTable(
                name: "UserFavoriteRecipes");

            migrationBuilder.DropTable(
                name: "UserRecipeViews");

            migrationBuilder.DropTable(
                name: "UserSaveRecipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_ImageId",
                table: "Recipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PermissionDomains",
                table: "PermissionDomains");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("104dfcfa-1ea8-e98c-86d6-2a54dfc76667"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("4946df8e-30a3-6ab7-5f45-bef28f0536bc"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("4aaf7650-f5b9-6640-a4da-f851a49e6d16"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("7bd0c333-0cc5-a866-0902-8d606e59e9de"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("b9e09fc1-bafc-f5aa-2396-91ca05ac7839"), new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("104dfcfa-1ea8-e98c-86d6-2a54dfc76667"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("4946df8e-30a3-6ab7-5f45-bef28f0536bc"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("4aaf7650-f5b9-6640-a4da-f851a49e6d16"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("7bd0c333-0cc5-a866-0902-8d606e59e9de"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("b9e09fc1-bafc-f5aa-2396-91ca05ac7839"), new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("104dfcfa-1ea8-e98c-86d6-2a54dfc76667"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("4946df8e-30a3-6ab7-5f45-bef28f0536bc"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("4aaf7650-f5b9-6640-a4da-f851a49e6d16"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("7bd0c333-0cc5-a866-0902-8d606e59e9de"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionActionId", "RoleId" },
                keyValues: new object[] { new Guid("b9e09fc1-bafc-f5aa-2396-91ca05ac7839"), new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728") });

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("104dfcfa-1ea8-e98c-86d6-2a54dfc76667"));

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("4946df8e-30a3-6ab7-5f45-bef28f0536bc"));

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("4aaf7650-f5b9-6640-a4da-f851a49e6d16"));

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("7bd0c333-0cc5-a866-0902-8d606e59e9de"));

            migrationBuilder.DeleteData(
                table: "PermissionActions",
                keyColumn: "Id",
                keyValue: new Guid("b9e09fc1-bafc-f5aa-2396-91ca05ac7839"));

            migrationBuilder.DeleteData(
                table: "PermissionDomains",
                keyColumn: "Id",
                keyValue: new Guid("6adf21b0-46ac-c454-54f4-6c77646e745f"));

            migrationBuilder.DeleteData(
                table: "PermissionDomains",
                keyColumn: "Id",
                keyValue: new Guid("f90072cc-a782-723a-e251-e25bc6ca5e6f"));

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Ration",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "ColorCode",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "IngredientCategories");

            migrationBuilder.RenameTable(
                name: "PermissionDomains",
                newName: "PermissionDomain");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Recipes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Instruction",
                table: "Recipes",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PermissionDomain",
                table: "PermissionDomain",
                column: "Id");

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
                    { new Guid("00000000-0000-0000-0000-000000000010"), "No measurable unit", "None", "" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategoryAssignments_CategoryId",
                table: "IngredientCategoryAssignments",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionActions_PermissionDomain_PermissionDomainId",
                table: "PermissionActions",
                column: "PermissionDomainId",
                principalTable: "PermissionDomain",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
