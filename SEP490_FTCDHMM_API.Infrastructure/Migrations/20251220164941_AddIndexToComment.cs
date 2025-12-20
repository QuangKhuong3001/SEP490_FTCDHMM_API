using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexToComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comments_RecipeId",
                table: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                newName: "IX_Comments_Parent");

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
                name: "IX_Comments_Recipe_Parent",
                table: "Comments",
                columns: new[] { "RecipeId", "ParentCommentId" });

            migrationBuilder.CreateIndex(
                name: "IX_CommentMentions_CommentId",
                table: "CommentMentions",
                column: "CommentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comments_Recipe_Parent",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_CommentMentions_CommentId",
                table: "CommentMentions");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_Parent",
                table: "Comments",
                newName: "IX_Comments_ParentCommentId");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 17, 19, 50, 5, 237, DateTimeKind.Utc).AddTicks(5518));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("133554ee-b8bf-0518-a055-4097baea7b64"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 17, 19, 50, 5, 237, DateTimeKind.Utc).AddTicks(7838));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("16a7239f-04ef-4ae8-3c3d-f7c91f625ade"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 17, 19, 50, 5, 237, DateTimeKind.Utc).AddTicks(7849));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("19f3c506-46ad-f9be-3a10-63dc2ed6a57e"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 17, 19, 50, 5, 237, DateTimeKind.Utc).AddTicks(7860));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("8443f632-4d26-96c3-6c99-cdb180c761f3"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 17, 19, 50, 5, 237, DateTimeKind.Utc).AddTicks(7854));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("b6cb3448-5f59-44b8-e69e-5a2e408ccd97"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 17, 19, 50, 5, 237, DateTimeKind.Utc).AddTicks(7865));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("c8f90ed8-cc93-7d51-8477-534ff99d0fd0"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 17, 19, 50, 5, 237, DateTimeKind.Utc).AddTicks(7870));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d238ef58-09be-5176-f430-16cdbfc0032a"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 17, 19, 50, 5, 237, DateTimeKind.Utc).AddTicks(7875));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("d5caeabc-0ca2-b778-f234-d5c084dd23cb"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 17, 19, 50, 5, 237, DateTimeKind.Utc).AddTicks(7844));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("edae6e4e-e3a4-ccd4-a2d4-81edf652d3f4"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 17, 19, 50, 5, 237, DateTimeKind.Utc).AddTicks(7828));

            migrationBuilder.UpdateData(
                table: "Labels",
                keyColumn: "Id",
                keyValue: new Guid("f4a0ea3a-98b1-3443-4739-f63803a841c8"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 17, 19, 50, 5, 237, DateTimeKind.Utc).AddTicks(7881));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("00edafe3-b047-5980-d0fa-da10f400c1e5"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 17, 19, 50, 5, 237, DateTimeKind.Utc).AddTicks(5832));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("1d6026ce-0dac-13ea-8b72-95f02b7620a7"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 17, 19, 50, 5, 237, DateTimeKind.Utc).AddTicks(5891));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("8ea665ca-b310-5ac6-c897-ff8b89f9f728"),
                column: "LastUpdatedUtc",
                value: new DateTime(2025, 12, 17, 19, 50, 5, 237, DateTimeKind.Utc).AddTicks(5881));

            migrationBuilder.CreateIndex(
                name: "IX_Comments_RecipeId",
                table: "Comments",
                column: "RecipeId");
        }
    }
}
