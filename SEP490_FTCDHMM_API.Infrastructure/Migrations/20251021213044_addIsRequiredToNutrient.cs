﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addIsRequiredToNutrient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Nutrients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "Nutrients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("3a9a556f-7285-4572-28aa-67447560ece8"),
                column: "Description",
                value: "Giúp tạo máu và duy trì hệ thần kinh.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("40d7e2f9-a5da-064c-fe4d-28febe860039"),
                column: "Description",
                value: "Chống oxy hóa, bảo vệ tế bào.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("422833be-5c43-e625-7a6a-6a74c32794a6"),
                column: "Description",
                value: "Chuyển hóa năng lượng, hỗ trợ thần kinh.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("4345a4c7-9cd2-6519-5892-9dcc40bb9ecc"),
                column: "Description",
                value: "Tăng cường miễn dịch, chống oxy hóa.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("4e465394-2d14-2a0a-7a00-5db0bc9e4597"),
                column: "Description",
                value: "Giúp tổng hợp hồng cầu, duy trì trao đổi chất.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("4e7a667e-4012-d80e-9276-1cd44d4e7fbd"),
                columns: new[] { "Description", "IsRequired" },
                values: new object[] { "Giúp xây dựng cơ bắp và tế bào.", true });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("55fb9dc4-6bf0-06b4-e2c5-ca786f557d38"),
                column: "Description",
                value: "Chống oxy hóa, tăng cường miễn dịch.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("5e08cf55-2b60-6f26-ef65-305553ffb09b"),
                column: "Description",
                value: "Giúp sáng mắt và tăng sức đề kháng.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("67dc4e3a-958b-9f2b-ba70-d7e6690b8f2d"),
                column: "Description",
                value: "Duy trì cân bằng nước và nhịp tim.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("73cd094d-61aa-61ce-d021-9ffa9b9ebbad"),
                columns: new[] { "Description", "IsRequired" },
                values: new object[] { "Tổng lượng chất béo trong thực phẩm.", true });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("7dd02ec7-0bde-e9d2-4f7b-99e3184f139e"),
                column: "Description",
                value: "Hỗ trợ miễn dịch, da, tóc và móng.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("7df9ddde-bcce-958a-2a38-85778c6cfb7b"),
                column: "Description",
                value: "Giúp đông máu và duy trì xương khỏe mạnh.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("88264feb-65c1-6808-c47c-44e3ebe1f725"),
                column: "Description",
                value: "Giúp hình thành xương và răng.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("968aface-8106-d49e-09dc-761ca6080887"),
                column: "Description",
                value: "Thành phần của huyết sắc tố (hemoglobin).");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("ac960903-ad9f-dfae-e0b3-35628565a3cb"),
                column: "Description",
                value: "Tốt cho da, mắt và hệ thần kinh.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("ba6906e3-9e16-e3df-06c5-f3b628919649"),
                column: "Description",
                value: "Giúp điều hòa nước và áp suất máu.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("bc5e858f-8aaa-e3f1-c7ae-bf691e5fa88e"),
                column: "Description",
                value: "Giúp chuyển hóa năng lượng, bảo vệ tim mạch.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c52f37b6-b8ba-c587-72d7-d3f5dc8044d6"),
                column: "Description",
                value: "Cần cho xương và não.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c8cd2a0b-6458-d98b-0ebf-0243cf575556"),
                column: "Description",
                value: "Giúp hấp thu canxi, tốt cho xương.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("d58dca3f-be87-c7d0-5396-223e9ced53a8"),
                columns: new[] { "Description", "IsRequired" },
                values: new object[] { "Tổng năng lượng cung cấp (Energy)", true });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("dbe42ec4-51b1-f98d-66fe-6fed6bdcad0a"),
                column: "Description",
                value: "Tổng lượng đường tự nhiên và thêm vào.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("e19cd21d-2c38-e38f-7c55-f643fd65daf9"),
                column: "Description",
                value: "Hỗ trợ tiêu hóa và giảm cholesterol.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("ed0c64a9-afc7-216a-a83e-8aebc743e462"),
                column: "Description",
                value: "Cần thiết cho xương và răng chắc khỏe.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("f2e0b30a-40ad-f850-5251-36fd00dc462e"),
                column: "Description",
                value: "Cholesterol trong thực phẩm.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("f3c5dea5-8442-1e88-a8bb-d71679c86ede"),
                column: "Description",
                value: "Quan trọng cho cơ và thần kinh.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("fa0f09a4-fbbd-3da5-76b0-748a0d87ce21"),
                column: "Description",
                value: "Tham gia hình thành tế bào máu và enzyme.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("feca7dbc-1254-74f3-c7e0-ff7b786515d0"),
                columns: new[] { "Description", "IsRequired" },
                values: new object[] { "Nguồn năng lượng chính của cơ thể.", true });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("ff39565c-1b2f-7db1-4f47-7b9ca86221f6"),
                column: "Description",
                value: "Quan trọng cho phụ nữ mang thai và tế bào mới.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Nutrients");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "Nutrients");

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
