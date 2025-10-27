using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class vtest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HealthGoalTargets_Nutrients_NutrientId",
                table: "HealthGoalTargets");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_Ingredients_IngredientsId",
                table: "RecipeIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_Recipes_RecipesId",
                table: "RecipeIngredients");

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
                name: "MinPer100",
                table: "IngredientNutrients",
                newName: "MinValue");

            migrationBuilder.RenameColumn(
                name: "MedianPer100g",
                table: "IngredientNutrients",
                newName: "MedianValue");

            migrationBuilder.RenameColumn(
                name: "MaxPer100",
                table: "IngredientNutrients",
                newName: "MaxValue");

            migrationBuilder.AddColumn<decimal>(
                name: "QuantityGram",
                table: "RecipeIngredients",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "MinValue",
                table: "HealthGoalTargets",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxValue",
                table: "HealthGoalTargets",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "MaxEnergyPct",
                table: "HealthGoalTargets",
                type: "decimal(6,4)",
                precision: 6,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MedianEnergyPct",
                table: "HealthGoalTargets",
                type: "decimal(6,4)",
                precision: 6,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MedianValue",
                table: "HealthGoalTargets",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinEnergyPct",
                table: "HealthGoalTargets",
                type: "decimal(6,4)",
                precision: 6,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TargetType",
                table: "HealthGoalTargets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Weight",
                table: "HealthGoalTargets",
                type: "decimal(9,3)",
                precision: 9,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

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

            migrationBuilder.CreateIndex(
                name: "IX_RecipeNutritionAggregates_NutrientId",
                table: "RecipeNutritionAggregates",
                column: "NutrientId");

            migrationBuilder.AddForeignKey(
                name: "FK_HealthGoalTargets_Nutrients_NutrientId",
                table: "HealthGoalTargets",
                column: "NutrientId",
                principalTable: "Nutrients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_HealthGoalTargets_Nutrients_NutrientId",
                table: "HealthGoalTargets");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_Ingredients_IngredientId",
                table: "RecipeIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_Recipes_RecipeId",
                table: "RecipeIngredients");

            migrationBuilder.DropTable(
                name: "RecipeNutritionAggregates");

            migrationBuilder.DropColumn(
                name: "QuantityGram",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "MaxEnergyPct",
                table: "HealthGoalTargets");

            migrationBuilder.DropColumn(
                name: "MedianEnergyPct",
                table: "HealthGoalTargets");

            migrationBuilder.DropColumn(
                name: "MedianValue",
                table: "HealthGoalTargets");

            migrationBuilder.DropColumn(
                name: "MinEnergyPct",
                table: "HealthGoalTargets");

            migrationBuilder.DropColumn(
                name: "TargetType",
                table: "HealthGoalTargets");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "HealthGoalTargets");

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
                newName: "MinPer100");

            migrationBuilder.RenameColumn(
                name: "MedianValue",
                table: "IngredientNutrients",
                newName: "MedianPer100g");

            migrationBuilder.RenameColumn(
                name: "MaxValue",
                table: "IngredientNutrients",
                newName: "MaxPer100");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinValue",
                table: "HealthGoalTargets",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxValue",
                table: "HealthGoalTargets",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HealthGoalTargets_Nutrients_NutrientId",
                table: "HealthGoalTargets",
                column: "NutrientId",
                principalTable: "Nutrients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
