using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addCookingStepImagesEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CookingSteps_Images_ImageId",
                table: "CookingSteps");

            migrationBuilder.DropIndex(
                name: "IX_CookingSteps_ImageId",
                table: "CookingSteps");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "CookingSteps");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Images",
                newName: "CreatedAtUTC");

            migrationBuilder.AlterColumn<string>(
                name: "Instruction",
                table: "CookingSteps",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.CreateTable(
                name: "CookingStepImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CookingStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CookingStepImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CookingStepImages_CookingSteps_CookingStepId",
                        column: x => x.CookingStepId,
                        principalTable: "CookingSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CookingStepImages_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Images",
                columns: new[] { "Id", "ContentType", "CreatedAtUTC", "Key", "UploadedById" },
                values: new object[] { new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"), "image/png", new DateTime(2025, 11, 13, 18, 51, 13, 856, DateTimeKind.Utc).AddTicks(1740), "images/default/no-image.png", null });

            migrationBuilder.InsertData(
                table: "Labels",
                columns: new[] { "Id", "ColorCode", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { new Guid("133554ee-b8bf-0518-a055-4097baea7b64"), "#2196F3", false, "Giàu đạm" },
                    { new Guid("16a7239f-04ef-4ae8-3c3d-f7c91f625ade"), "#8BC34A", false, "Thuần chay" },
                    { new Guid("19f3c506-46ad-f9be-3a10-63dc2ed6a57e"), "#FF9800", false, "Không gluten" },
                    { new Guid("8443f632-4d26-96c3-6c99-cdb180c761f3"), "#CDDC39", false, "Chay" },
                    { new Guid("b6cb3448-5f59-44b8-e69e-5a2e408ccd97"), "#FFC107", false, "Món nhanh" },
                    { new Guid("c8f90ed8-cc93-7d51-8477-534ff99d0fd0"), "#00BCD4", false, "Ít béo" },
                    { new Guid("d238ef58-09be-5176-f430-16cdbfc0032a"), "#795548", false, "Keto" },
                    { new Guid("d5caeabc-0ca2-b778-f234-d5c084dd23cb"), "#9C27B0", false, "Ít tinh bột" },
                    { new Guid("edae6e4e-e3a4-ccd4-a2d4-81edf652d3f4"), "#4CAF50", false, "Lành mạnh" },
                    { new Guid("f4a0ea3a-98b1-3443-4739-f63803a841c8"), "#FF5722", false, "Phù hợp cho người tiểu đường" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CookingStepImages_CookingStepId",
                table: "CookingStepImages",
                column: "CookingStepId");

            migrationBuilder.CreateIndex(
                name: "IX_CookingStepImages_ImageId",
                table: "CookingStepImages",
                column: "ImageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CookingStepImages");

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"));

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("133554ee-b8bf-0518-a055-4097baea7b64"));

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("16a7239f-04ef-4ae8-3c3d-f7c91f625ade"));

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("19f3c506-46ad-f9be-3a10-63dc2ed6a57e"));

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("8443f632-4d26-96c3-6c99-cdb180c761f3"));

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("b6cb3448-5f59-44b8-e69e-5a2e408ccd97"));

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("c8f90ed8-cc93-7d51-8477-534ff99d0fd0"));

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d238ef58-09be-5176-f430-16cdbfc0032a"));

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d5caeabc-0ca2-b778-f234-d5c084dd23cb"));

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("edae6e4e-e3a4-ccd4-a2d4-81edf652d3f4"));

            migrationBuilder.DeleteData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("f4a0ea3a-98b1-3443-4739-f63803a841c8"));

            migrationBuilder.RenameColumn(
                name: "CreatedAtUTC",
                table: "Images",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "Instruction",
                table: "CookingSteps",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<Guid>(
                name: "ImageId",
                table: "CookingSteps",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CookingSteps_ImageId",
                table: "CookingSteps",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_CookingSteps_Images_ImageId",
                table: "CookingSteps",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
