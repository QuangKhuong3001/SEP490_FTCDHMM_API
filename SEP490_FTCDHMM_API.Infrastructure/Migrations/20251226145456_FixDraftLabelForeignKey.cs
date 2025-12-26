using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixDraftLabelForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Labels_DraftRecipes_DraftRecipeId",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Labels_DraftRecipeId",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "DraftRecipeId",
                table: "Labels");

            migrationBuilder.CreateTable(
                name: "DraftRecipeLabels",
                columns: table => new
                {
                    DraftRecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftRecipeLabels", x => new { x.DraftRecipeId, x.LabelId });
                    table.ForeignKey(
                        name: "FK_DraftRecipeLabels_DraftRecipes_DraftRecipeId",
                        column: x => x.DraftRecipeId,
                        principalTable: "DraftRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DraftRecipeLabels_Labels_LabelId",
                        column: x => x.LabelId,
                        principalTable: "Labels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(4353));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("133554ee-b8bf-0518-a055-4097baea7b64"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6432));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("16a7239f-04ef-4ae8-3c3d-f7c91f625ade"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6471));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("19f3c506-46ad-f9be-3a10-63dc2ed6a57e"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6483));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("8443f632-4d26-96c3-6c99-cdb180c761f3"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6477));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("b6cb3448-5f59-44b8-e69e-5a2e408ccd97"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6488));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("c8f90ed8-cc93-7d51-8477-534ff99d0fd0"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6493));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d238ef58-09be-5176-f430-16cdbfc0032a"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6498));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d5caeabc-0ca2-b778-f234-d5c084dd23cb"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6438));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("edae6e4e-e3a4-ccd4-a2d4-81edf652d3f4"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6424));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("f4a0ea3a-98b1-3443-4739-f63803a841c8"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(6504));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(4622));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(4673));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 26, 14, 54, 56, 34, DateTimeKind.Utc).AddTicks(4665));

            migrationBuilder.CreateIndex(
                name: "IX_DraftRecipeLabels_LabelId",
                table: "DraftRecipeLabels",
                column: "LabelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DraftRecipeLabels");

            migrationBuilder.AddColumn<Guid>(
                name: "DraftRecipeId",
                table: "Labels",
                type: "uniqueidentifier",
                nullable: true);

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
                columns: new[] { "DraftRecipeId", "LastUpdatedUtc" },
                values: new object[] { null, new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6563) });

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("16a7239f-04ef-4ae8-3c3d-f7c91f625ade"),
                columns: new[] { "DraftRecipeId", "LastUpdatedUtc" },
                values: new object[] { null, new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6574) });

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("19f3c506-46ad-f9be-3a10-63dc2ed6a57e"),
                columns: new[] { "DraftRecipeId", "LastUpdatedUtc" },
                values: new object[] { null, new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6584) });

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("8443f632-4d26-96c3-6c99-cdb180c761f3"),
                columns: new[] { "DraftRecipeId", "LastUpdatedUtc" },
                values: new object[] { null, new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6578) });

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("b6cb3448-5f59-44b8-e69e-5a2e408ccd97"),
                columns: new[] { "DraftRecipeId", "LastUpdatedUtc" },
                values: new object[] { null, new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6589) });

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("c8f90ed8-cc93-7d51-8477-534ff99d0fd0"),
                columns: new[] { "DraftRecipeId", "LastUpdatedUtc" },
                values: new object[] { null, new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6594) });

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d238ef58-09be-5176-f430-16cdbfc0032a"),
                columns: new[] { "DraftRecipeId", "LastUpdatedUtc" },
                values: new object[] { null, new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6599) });

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d5caeabc-0ca2-b778-f234-d5c084dd23cb"),
                columns: new[] { "DraftRecipeId", "LastUpdatedUtc" },
                values: new object[] { null, new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6569) });

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("edae6e4e-e3a4-ccd4-a2d4-81edf652d3f4"),
                columns: new[] { "DraftRecipeId", "LastUpdatedUtc" },
                values: new object[] { null, new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6550) });

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("f4a0ea3a-98b1-3443-4739-f63803a841c8"),
                columns: new[] { "DraftRecipeId", "LastUpdatedUtc" },
                values: new object[] { null, new DateTime(2025, 12, 26, 14, 50, 57, 999, DateTimeKind.Utc).AddTicks(6605) });

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
                name: "IX_Labels_DraftRecipeId",
                table: "Labels",
                column: "DraftRecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_DraftRecipes_DraftRecipeId",
                table: "Labels",
                column: "DraftRecipeId",
                principalTable: "DraftRecipes",
                principalColumn: "Id");
        }
    }
}
