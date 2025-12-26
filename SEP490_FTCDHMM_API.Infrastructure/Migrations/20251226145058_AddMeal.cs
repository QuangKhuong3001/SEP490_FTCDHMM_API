using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMeal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSaveRecipes_RecipeId",
                table: "UserSaveRecipes");

            migrationBuilder.DropIndex(
                name: "IX_UserIngredientRestrictions_UserId",
                table: "UserIngredientRestrictions");

            migrationBuilder.DropIndex(
                name: "IX_UserHealthMetrics_UserId",
                table: "UserHealthMetrics");

            migrationBuilder.DropIndex(
                name: "IX_UserHealthGoals_UserId",
                table: "UserHealthGoals");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_RecipeId",
                table: "Ratings");

            migrationBuilder.RenameIndex(
                name: "IX_UserIngredientRestrictions_IngredientId",
                table: "UserIngredientRestrictions",
                newName: "IX_DietRestrictions_Ingredient");

            migrationBuilder.RenameIndex(
                name: "IX_UserIngredientRestrictions_IngredientCategoryId",
                table: "UserIngredientRestrictions",
                newName: "IX_DietRestrictions_Category");

            migrationBuilder.RenameIndex(
                name: "IX_Recipes_AuthorId",
                table: "Recipes",
                newName: "IX_Recipes_Author");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Recipes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValueSql: "'POSTED'",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValueSql: "'POSTED'");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(4148));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("133554ee-b8bf-0518-a055-4097baea7b64"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6563));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("16a7239f-04ef-4ae8-3c3d-f7c91f625ade"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6574));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("19f3c506-46ad-f9be-3a10-63dc2ed6a57e"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6584));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("8443f632-4d26-96c3-6c99-cdb180c761f3"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6578));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("b6cb3448-5f59-44b8-e69e-5a2e408ccd97"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6589));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("c8f90ed8-cc93-7d51-8477-534ff99d0fd0"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6594));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d238ef58-09be-5176-f430-16cdbfc0032a"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6599));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d5caeabc-0ca2-b778-f234-d5c084dd23cb"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6569));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("edae6e4e-e3a4-ccd4-a2d4-81edf652d3f4"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6550));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("f4a0ea3a-98b1-3443-4739-f63803a841c8"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6605));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(4490));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(4550));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(4542));

            migrationBuilder.CreateIndex(
                name: "IX_SaveRecipes_Recipe_Time",
                table: "UserSaveRecipes",
                columns: new[] { "RecipeId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_DietRestrictions_User_Active",
                table: "UserIngredientRestrictions",
                columns: new[] { "UserId", "ExpiredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_UserHealthMetrics_User_Time",
                table: "UserHealthMetrics",
                columns: new[] { "UserId", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserHealthGoals_Active",
                table: "UserHealthGoals",
                columns: new[] { "UserId", "StartedAtUtc", "ExpiredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_Status_Updated",
                table: "Recipes",
                columns: new[] { "Status", "UpdatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_Recipe_Time",
                table: "Ratings",
                columns: new[] { "RecipeId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_Recipe_Created",
                table: "Comments",
                columns: new[] { "RecipeId", "CreatedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SaveRecipes_Recipe_Time",
                table: "UserSaveRecipes");

            migrationBuilder.DropIndex(
                name: "IX_DietRestrictions_User_Active",
                table: "UserIngredientRestrictions");

            migrationBuilder.DropIndex(
                name: "IX_UserHealthMetrics_User_Time",
                table: "UserHealthMetrics");

            migrationBuilder.DropIndex(
                name: "IX_UserHealthGoals_Active",
                table: "UserHealthGoals");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_Status_Updated",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_Recipe_Time",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Comments_Recipe_Created",
                table: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_DietRestrictions_Ingredient",
                table: "UserIngredientRestrictions",
                newName: "IX_UserIngredientRestrictions_IngredientId");

            migrationBuilder.RenameIndex(
                name: "IX_DietRestrictions_Category",
                table: "UserIngredientRestrictions",
                newName: "IX_UserIngredientRestrictions_IngredientCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Recipes_Author",
                table: "Recipes",
                newName: "IX_Recipes_AuthorId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValueSql: "'POSTED'",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldDefaultValueSql: "'POSTED'");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 20, 16, 49, 40, 941, DateTimeKind.Utc).AddTicks(5380));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("133554ee-b8bf-0518-a055-4097baea7b64"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 20, 16, 49, 40, 941, DateTimeKind.Utc).AddTicks(7347));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("16a7239f-04ef-4ae8-3c3d-f7c91f625ade"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 20, 16, 49, 40, 941, DateTimeKind.Utc).AddTicks(7357));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("19f3c506-46ad-f9be-3a10-63dc2ed6a57e"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 20, 16, 49, 40, 941, DateTimeKind.Utc).AddTicks(7368));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("8443f632-4d26-96c3-6c99-cdb180c761f3"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 20, 16, 49, 40, 941, DateTimeKind.Utc).AddTicks(7363));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("b6cb3448-5f59-44b8-e69e-5a2e408ccd97"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 20, 16, 49, 40, 941, DateTimeKind.Utc).AddTicks(7373));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("c8f90ed8-cc93-7d51-8477-534ff99d0fd0"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 20, 16, 49, 40, 941, DateTimeKind.Utc).AddTicks(7378));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d238ef58-09be-5176-f430-16cdbfc0032a"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 20, 16, 49, 40, 941, DateTimeKind.Utc).AddTicks(7384));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d5caeabc-0ca2-b778-f234-d5c084dd23cb"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 20, 16, 49, 40, 941, DateTimeKind.Utc).AddTicks(7352));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("edae6e4e-e3a4-ccd4-a2d4-81edf652d3f4"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 20, 16, 49, 40, 941, DateTimeKind.Utc).AddTicks(7337));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("f4a0ea3a-98b1-3443-4739-f63803a841c8"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 20, 16, 49, 40, 941, DateTimeKind.Utc).AddTicks(7413));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 20, 16, 49, 40, 941, DateTimeKind.Utc).AddTicks(5641));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 20, 16, 49, 40, 941, DateTimeKind.Utc).AddTicks(5684));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 20, 16, 49, 40, 941, DateTimeKind.Utc).AddTicks(5677));

            migrationBuilder.CreateIndex(
                name: "IX_UserSaveRecipes_RecipeId",
                table: "UserSaveRecipes",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserIngredientRestrictions_UserId",
                table: "UserIngredientRestrictions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHealthMetrics_UserId",
                table: "UserHealthMetrics",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHealthGoals_UserId",
                table: "UserHealthGoals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_RecipeId",
                table: "Ratings",
                column: "RecipeId");
        }
    }
}
