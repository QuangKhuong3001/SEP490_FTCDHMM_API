using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addImage_updateAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailOtp_AppUser_UserId",
                table: "EmailOtp");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "EmailOtp",
                newName: "SentToId");

            migrationBuilder.RenameIndex(
                name: "IX_EmailOtp_UserId",
                table: "EmailOtp",
                newName: "IX_EmailOtp_SentToId");

            migrationBuilder.AddColumn<Guid>(
                name: "AvatarId",
                table: "AppUser",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UploadedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Image_AppUser_UploadedById",
                        column: x => x.UploadedById,
                        principalTable: "AppUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_AvatarId",
                table: "AppUser",
                column: "AvatarId",
                unique: true,
                filter: "[AvatarId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Image_UploadedById",
                table: "Image",
                column: "UploadedById");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUser_Image_AvatarId",
                table: "AppUser",
                column: "AvatarId",
                principalTable: "Image",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailOtp_AppUser_SentToId",
                table: "EmailOtp",
                column: "SentToId",
                principalTable: "AppUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUser_Image_AvatarId",
                table: "AppUser");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailOtp_AppUser_SentToId",
                table: "EmailOtp");

            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropIndex(
                name: "IX_AppUser_AvatarId",
                table: "AppUser");

            migrationBuilder.DropColumn(
                name: "AvatarId",
                table: "AppUser");

            migrationBuilder.RenameColumn(
                name: "SentToId",
                table: "EmailOtp",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EmailOtp_SentToId",
                table: "EmailOtp",
                newName: "IX_EmailOtp_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailOtp_AppUser_UserId",
                table: "EmailOtp",
                column: "UserId",
                principalTable: "AppUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
