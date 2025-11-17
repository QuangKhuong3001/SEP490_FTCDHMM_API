using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addDraftEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DraftRecipeId",
                table: "Labels",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DraftRecipes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Difficulty = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CookTime = table.Column<int>(type: "int", nullable: false),
                    Ration = table.Column<int>(type: "int", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftRecipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DraftRecipes_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DraftRecipes_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DraftCookingSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Instruction = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    DraftRecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftCookingSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DraftCookingSteps_DraftRecipes_DraftRecipeId",
                        column: x => x.DraftRecipeId,
                        principalTable: "DraftRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DraftRecipeIngredients",
                columns: table => new
                {
                    DraftRecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuantityGram = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftRecipeIngredients", x => new { x.DraftRecipeId, x.IngredientId });
                    table.ForeignKey(
                        name: "FK_DraftRecipeIngredients_DraftRecipes_DraftRecipeId",
                        column: x => x.DraftRecipeId,
                        principalTable: "DraftRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DraftRecipeIngredients_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DraftRecipeUserTags",
                columns: table => new
                {
                    DraftRecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaggedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftRecipeUserTags", x => new { x.DraftRecipeId, x.TaggedUserId });
                    table.ForeignKey(
                        name: "FK_DraftRecipeUserTags_DraftRecipes_DraftRecipeId",
                        column: x => x.DraftRecipeId,
                        principalTable: "DraftRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DraftRecipeUserTags_Users_TaggedUserId",
                        column: x => x.TaggedUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DraftCookingStepImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DraftCookingStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftCookingStepImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DraftCookingStepImages_DraftCookingSteps_DraftCookingStepId",
                        column: x => x.DraftCookingStepId,
                        principalTable: "DraftCookingSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DraftCookingStepImages_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 11, 17, 3, 48, 55, 95, DateTimeKind.Utc).AddTicks(1573));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("133554ee-b8bf-0518-a055-4097baea7b64"),
                column: "DraftRecipeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("16a7239f-04ef-4ae8-3c3d-f7c91f625ade"),
                column: "DraftRecipeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("19f3c506-46ad-f9be-3a10-63dc2ed6a57e"),
                column: "DraftRecipeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("8443f632-4d26-96c3-6c99-cdb180c761f3"),
                column: "DraftRecipeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("b6cb3448-5f59-44b8-e69e-5a2e408ccd97"),
                column: "DraftRecipeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("c8f90ed8-cc93-7d51-8477-534ff99d0fd0"),
                column: "DraftRecipeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d238ef58-09be-5176-f430-16cdbfc0032a"),
                column: "DraftRecipeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d5caeabc-0ca2-b778-f234-d5c084dd23cb"),
                column: "DraftRecipeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("edae6e4e-e3a4-ccd4-a2d4-81edf652d3f4"),
                column: "DraftRecipeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("f4a0ea3a-98b1-3443-4739-f63803a841c8"),
                column: "DraftRecipeId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Labels_DraftRecipeId",
                table: "Labels",
                column: "DraftRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftCookingStepImages_DraftCookingStepId",
                table: "DraftCookingStepImages",
                column: "DraftCookingStepId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftCookingStepImages_ImageId",
                table: "DraftCookingStepImages",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftCookingSteps_DraftRecipeId",
                table: "DraftCookingSteps",
                column: "DraftRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftRecipeIngredients_IngredientId",
                table: "DraftRecipeIngredients",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftRecipes_AuthorId",
                table: "DraftRecipes",
                column: "AuthorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DraftRecipes_ImageId",
                table: "DraftRecipes",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftRecipeUserTags_TaggedUserId",
                table: "DraftRecipeUserTags",
                column: "TaggedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_DraftRecipes_DraftRecipeId",
                table: "Labels",
                column: "DraftRecipeId",
                principalTable: "DraftRecipes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Labels_DraftRecipes_DraftRecipeId",
                table: "Labels");

            migrationBuilder.DropTable(
                name: "DraftCookingStepImages");

            migrationBuilder.DropTable(
                name: "DraftRecipeIngredients");

            migrationBuilder.DropTable(
                name: "DraftRecipeUserTags");

            migrationBuilder.DropTable(
                name: "DraftCookingSteps");

            migrationBuilder.DropTable(
                name: "DraftRecipes");

            migrationBuilder.DropIndex(
                name: "IX_Labels_DraftRecipeId",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "DraftRecipeId",
                table: "Labels");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 11, 13, 21, 2, 30, 888, DateTimeKind.Utc).AddTicks(8569));
        }
    }
}
