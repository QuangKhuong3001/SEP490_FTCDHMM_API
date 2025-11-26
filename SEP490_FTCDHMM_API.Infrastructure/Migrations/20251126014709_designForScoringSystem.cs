using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class designForScoringSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HealthGoalTargets_HealthGoals_HealthGoalId",
                table: "HealthGoalTargets");

            migrationBuilder.DropForeignKey(
                name: "FK_HealthGoalTargets_Nutrients_NutrientId",
                table: "HealthGoalTargets");

            migrationBuilder.DropTable(
                name: "CustomHealthGoalTargets");

            migrationBuilder.DropColumn(
                name: "NumberOfRatings",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "PopularityScore",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "SearchCount",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "UsageFrequency",
                table: "Ingredients");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "Recipes",
                newName: "AvgRating");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "UserHealthGoals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValueSql: "'CUSTOM'");

            migrationBuilder.AlterColumn<string>(
                name: "Difficulty",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValueSql: "'MEDIUM'",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<decimal>(
                name: "Calories",
                table: "Recipes",
                type: "decimal(10,3)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,3)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RatingCount",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "HealthGoalTargets",
                type: "decimal(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,3)",
                oldPrecision: 9,
                oldScale: 3);

            migrationBuilder.AlterColumn<string>(
                name: "TargetType",
                table: "HealthGoalTargets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValueSql: "'ABSOLUTE'",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<decimal>(
                name: "MinValue",
                table: "HealthGoalTargets",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MinEnergyPct",
                table: "HealthGoalTargets",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,4)",
                oldPrecision: 6,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxValue",
                table: "HealthGoalTargets",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxEnergyPct",
                table: "HealthGoalTargets",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,4)",
                oldPrecision: 6,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "HealthGoalId",
                table: "HealthGoalTargets",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomHealthGoalId",
                table: "HealthGoalTargets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 11, 26, 1, 47, 9, 549, DateTimeKind.Utc).AddTicks(5788));

            migrationBuilder.CreateIndex(
                name: "IX_HealthGoalTargets_CustomHealthGoalId",
                table: "HealthGoalTargets",
                column: "CustomHealthGoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_HealthGoalTargets_CustomHealthGoals_CustomHealthGoalId",
                table: "HealthGoalTargets",
                column: "CustomHealthGoalId",
                principalTable: "CustomHealthGoals",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HealthGoalTargets_HealthGoals_HealthGoalId",
                table: "HealthGoalTargets",
                column: "HealthGoalId",
                principalTable: "HealthGoals",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HealthGoalTargets_Nutrients_NutrientId",
                table: "HealthGoalTargets",
                column: "NutrientId",
                principalTable: "Nutrients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HealthGoalTargets_CustomHealthGoals_CustomHealthGoalId",
                table: "HealthGoalTargets");

            migrationBuilder.DropForeignKey(
                name: "FK_HealthGoalTargets_HealthGoals_HealthGoalId",
                table: "HealthGoalTargets");

            migrationBuilder.DropForeignKey(
                name: "FK_HealthGoalTargets_Nutrients_NutrientId",
                table: "HealthGoalTargets");

            migrationBuilder.DropIndex(
                name: "IX_HealthGoalTargets_CustomHealthGoalId",
                table: "HealthGoalTargets");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "UserHealthGoals");

            migrationBuilder.DropColumn(
                name: "RatingCount",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "CustomHealthGoalId",
                table: "HealthGoalTargets");

            migrationBuilder.RenameColumn(
                name: "AvgRating",
                table: "Recipes",
                newName: "Rating");

            migrationBuilder.AlterColumn<string>(
                name: "Difficulty",
                table: "Recipes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValueSql: "'MEDIUM'");

            migrationBuilder.AlterColumn<decimal>(
                name: "Calories",
                table: "Recipes",
                type: "decimal(10,3)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,3)");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfRatings",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PopularityScore",
                table: "Ingredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "SearchCount",
                table: "Ingredients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsageFrequency",
                table: "Ingredients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "HealthGoalTargets",
                type: "decimal(9,3)",
                precision: 9,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)",
                oldPrecision: 6,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "TargetType",
                table: "HealthGoalTargets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValueSql: "'ABSOLUTE'");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinValue",
                table: "HealthGoalTargets",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MinEnergyPct",
                table: "HealthGoalTargets",
                type: "decimal(6,4)",
                precision: 6,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxValue",
                table: "HealthGoalTargets",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxEnergyPct",
                table: "HealthGoalTargets",
                type: "decimal(6,4)",
                precision: 6,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "HealthGoalId",
                table: "HealthGoalTargets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CustomHealthGoalTargets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomHealthGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NutrientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaxEnergyPct = table.Column<decimal>(type: "decimal(6,4)", precision: 6, scale: 4, nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    MinEnergyPct = table.Column<decimal>(type: "decimal(6,4)", precision: 6, scale: 4, nullable: true),
                    MinValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    TargetType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 11, 25, 2, 54, 18, 489, DateTimeKind.Utc).AddTicks(8081));

            migrationBuilder.CreateIndex(
                name: "IX_CustomHealthGoalTargets_CustomHealthGoalId",
                table: "CustomHealthGoalTargets",
                column: "CustomHealthGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomHealthGoalTargets_NutrientId",
                table: "CustomHealthGoalTargets",
                column: "NutrientId");

            migrationBuilder.AddForeignKey(
                name: "FK_HealthGoalTargets_HealthGoals_HealthGoalId",
                table: "HealthGoalTargets",
                column: "HealthGoalId",
                principalTable: "HealthGoals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HealthGoalTargets_Nutrients_NutrientId",
                table: "HealthGoalTargets",
                column: "NutrientId",
                principalTable: "Nutrients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
