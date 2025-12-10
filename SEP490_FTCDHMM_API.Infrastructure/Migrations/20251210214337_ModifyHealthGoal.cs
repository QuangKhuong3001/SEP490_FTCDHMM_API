using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyHealthGoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Weight",
                table: "HealthGoalTargets",
                type: "int",
                precision: 6,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)",
                oldPrecision: 6,
                oldScale: 2);

            migrationBuilder.AddColumn<string>(
                name: "UpperName",
                table: "HealthGoals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 10, 21, 43, 37, 509, DateTimeKind.Utc).AddTicks(6548));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpperName",
                table: "HealthGoals");

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "HealthGoalTargets",
                type: "decimal(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldPrecision: 6,
                oldScale: 2);

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 10, 19, 34, 41, 633, DateTimeKind.Utc).AddTicks(4350));
        }
    }
}
