using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserDietRestriction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserHealthMetric_Users_UserId",
                table: "UserHealthMetric");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserHealthMetric",
                table: "UserHealthMetric");

            migrationBuilder.RenameTable(
                name: "UserHealthMetric",
                newName: "UserHealthMetrics");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "Labels",
                newName: "IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_UserHealthMetric_UserId",
                table: "UserHealthMetrics",
                newName: "IX_UserHealthMetrics_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserHealthMetrics",
                table: "UserHealthMetrics",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserIngredientRestrictions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IngredientCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "'ALLERGY'"),
                    Notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ExpiredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIngredientRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserIngredientRestrictions_IngredientCategories_IngredientCategoryId",
                        column: x => x.IngredientCategoryId,
                        principalTable: "IngredientCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserIngredientRestrictions_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserIngredientRestrictions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserIngredientRestrictions_IngredientCategoryId",
                table: "UserIngredientRestrictions",
                column: "IngredientCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserIngredientRestrictions_IngredientId",
                table: "UserIngredientRestrictions",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_UserIngredientRestrictions_UserId",
                table: "UserIngredientRestrictions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserHealthMetrics_Users_UserId",
                table: "UserHealthMetrics",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserHealthMetrics_Users_UserId",
                table: "UserHealthMetrics");

            migrationBuilder.DropTable(
                name: "UserIngredientRestrictions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserHealthMetrics",
                table: "UserHealthMetrics");

            migrationBuilder.RenameTable(
                name: "UserHealthMetrics",
                newName: "UserHealthMetric");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Labels",
                newName: "isDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_UserHealthMetrics_UserId",
                table: "UserHealthMetric",
                newName: "IX_UserHealthMetric_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserHealthMetric",
                table: "UserHealthMetric",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserHealthMetric_Users_UserId",
                table: "UserHealthMetric",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
