using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("d58dca3f-be87-c7d0-5396-223e9ced53a8"));

            migrationBuilder.RenameColumn(
                name: "Min",
                table: "IngredientNutrients",
                newName: "MinPer100");

            migrationBuilder.RenameColumn(
                name: "Median",
                table: "IngredientNutrients",
                newName: "MedianPer100g");

            migrationBuilder.RenameColumn(
                name: "Max",
                table: "IngredientNutrients",
                newName: "MaxPer100");

            migrationBuilder.AlterColumn<decimal>(
                name: "CookTime",
                table: "Recipes",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(double),
                oldType: "float(10)",
                oldPrecision: 10,
                oldScale: 2);

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
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
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
                name: "CustomHealthGoalTargets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomHealthGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NutrientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MaxValue = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HealthGoalTargets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HealthGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NutrientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MaxValue = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
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
                        onDelete: ReferentialAction.Restrict);
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
                name: "IX_HealthGoalTargets_HealthGoalId",
                table: "HealthGoalTargets",
                column: "HealthGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthGoalTargets_NutrientId",
                table: "HealthGoalTargets",
                column: "NutrientId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHealthGoals_HealthGoalId",
                table: "UserHealthGoals",
                column: "HealthGoalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomHealthGoalTargets");

            migrationBuilder.DropTable(
                name: "HealthGoalTargets");

            migrationBuilder.DropTable(
                name: "UserHealthGoals");

            migrationBuilder.DropTable(
                name: "CustomHealthGoals");

            migrationBuilder.DropTable(
                name: "HealthGoals");

            migrationBuilder.DropColumn(
                name: "Calories",
                table: "Ingredients");

            migrationBuilder.RenameColumn(
                name: "MinPer100",
                table: "IngredientNutrients",
                newName: "Min");

            migrationBuilder.RenameColumn(
                name: "MedianPer100g",
                table: "IngredientNutrients",
                newName: "Median");

            migrationBuilder.RenameColumn(
                name: "MaxPer100",
                table: "IngredientNutrients",
                newName: "Max");

            migrationBuilder.AlterColumn<double>(
                name: "CookTime",
                table: "Recipes",
                type: "float(10)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldDefaultValue: 0m);

            migrationBuilder.InsertData(
                table: "Nutrients",
                columns: new[] { "Id", "Description", "IsRequired", "Name", "UnitId", "VietnameseName" },
                values: new object[] { new Guid("d58dca3f-be87-c7d0-5396-223e9ced53a8"), "Tổng năng lượng cung cấp (Energy)", true, "Calories", new Guid("93d2464a-59b3-7951-21fe-5dc36fe13c45"), "Năng lượng" });
        }
    }
}
