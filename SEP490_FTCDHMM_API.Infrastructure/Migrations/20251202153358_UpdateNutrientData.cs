using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SEP490_FTCDHMM_API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNutrientData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("5e08cf55-2b60-6f26-ef65-305553ffb09b"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("73cd094d-61aa-61ce-d021-9ffa9b9ebbad"));

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 2, 15, 33, 57, 958, DateTimeKind.Utc).AddTicks(4200));

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("3a9a556f-7285-4572-28aa-67447560ece8"),
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("40d7e2f9-a5da-064c-fe4d-28febe860039"),
                columns: new[] { "Description", "Name" },
                values: new object[] { null, "Vitamin E (alpha-tocopherol)" });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("422833be-5c43-e625-7a6a-6a74c32794a6"),
                columns: new[] { "Description", "Name" },
                values: new object[] { null, "Thiamin (B1)" });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("4345a4c7-9cd2-6519-5892-9dcc40bb9ecc"),
                column: "Description",
                value: "Tăng miễn dịch, chống oxy hóa.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("4e465394-2d14-2a0a-7a00-5db0bc9e4597"),
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("4e7a667e-4012-d80e-9276-1cd44d4e7fbd"),
                column: "Description",
                value: "Hỗ trợ xây dựng mô, cơ, enzyme và hormone.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("55fb9dc4-6bf0-06b4-e2c5-ca786f557d38"),
                column: "Description",
                value: "Chống oxy hóa mạnh.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("67dc4e3a-958b-9f2b-ba70-d7e6690b8f2d"),
                column: "Description",
                value: "Điện giải quan trọng cho cơ và tim.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("7dd02ec7-0bde-e9d2-4f7b-99e3184f139e"),
                column: "Description",
                value: "Quan trọng cho miễn dịch và enzyme.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("7df9ddde-bcce-958a-2a38-85778c6cfb7b"),
                columns: new[] { "Description", "Name" },
                values: new object[] { null, "Vitamin K (phylloquinone)" });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("88264feb-65c1-6808-c47c-44e3ebe1f725"),
                column: "Description",
                value: "Hình thành xương và DNA.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("968aface-8106-d49e-09dc-761ca6080887"),
                column: "Description",
                value: "Thành phần của hemoglobin, vận chuyển oxy.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("ac960903-ad9f-dfae-e0b3-35628565a3cb"),
                columns: new[] { "Description", "Name" },
                values: new object[] { null, "Riboflavin (B2)" });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("ba6906e3-9e16-e3df-06c5-f3b628919649"),
                column: "Description",
                value: "Điều hòa nước và huyết áp.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("bc5e858f-8aaa-e3f1-c7ae-bf691e5fa88e"),
                columns: new[] { "Description", "Name" },
                values: new object[] { null, "Niacin (B3)" });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c52f37b6-b8ba-c587-72d7-d3f5dc8044d6"),
                column: "Description",
                value: "Tham gia trao đổi xương và enzyme.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c8cd2a0b-6458-d98b-0ebf-0243cf575556"),
                columns: new[] { "Description", "Name", "UnitId" },
                values: new object[] { null, "Vitamin D (D2 + D3)", new Guid("c75b9051-0442-cf83-bf9d-c7c30b1413ad") });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("dbe42ec4-51b1-f98d-66fe-6fed6bdcad0a"),
                columns: new[] { "Description", "Name", "VietnameseName" },
                values: new object[] { "Toàn bộ lượng đường tự nhiên và thêm vào.", "Sugars, total", "Tổng đường" });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("e19cd21d-2c38-e38f-7c55-f643fd65daf9"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Hỗ trợ tiêu hóa và ổn định đường huyết.", "Fiber, total dietary" });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("ed0c64a9-afc7-216a-a83e-8aebc743e462"),
                column: "Description",
                value: "Quan trọng cho xương, răng và truyền tín hiệu thần kinh.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("f2e0b30a-40ad-f850-5251-36fd00dc462e"),
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("f3c5dea5-8442-1e88-a8bb-d71679c86ede"),
                column: "Description",
                value: "Tham gia hơn 300 phản ứng enzyme.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("fa0f09a4-fbbd-3da5-76b0-748a0d87ce21"),
                column: "Description",
                value: "Tạo máu và enzyme.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("feca7dbc-1254-74f3-c7e0-ff7b786515d0"),
                columns: new[] { "Description", "Name", "VietnameseName" },
                values: new object[] { "Nguồn năng lượng chính cho cơ thể.", "Carbohydrate, by difference", "Carbohydrate" });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("ff39565c-1b2f-7db1-4f47-7b9ca86221f6"),
                columns: new[] { "Description", "Name" },
                values: new object[] { null, "Folate, total" });

            migrationBuilder.InsertData(
                table: "Nutrients",
                columns: new[] { "Id", "Description", "Name", "UnitId", "VietnameseName" },
                values: new object[,]
                {
                    { new Guid("0138bdbe-6512-9ccc-a332-791054106ef5"), "Khoáng tổng hợp còn lại sau khi đốt mẫu.", "Ash", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Tro" },
                    { new Guid("075edf8a-e1ec-4275-1f20-4aed6eed19d4"), null, "Carotene, beta", new Guid("c75b9051-0442-cf83-bf9d-c7c30b1413ad"), "Beta-carotene" },
                    { new Guid("08346647-35b8-ea15-1f06-fd61d09d88c4"), null, "Phenylalanine", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit amin Phenylalanine" },
                    { new Guid("0a259dfb-f18b-7a8f-2fec-33630aac3ea1"), null, "Vitamin A, RAE", new Guid("c75b9051-0442-cf83-bf9d-c7c30b1413ad"), "Vitamin A (RAE)" },
                    { new Guid("0df85a80-795d-fe5a-e1c4-ffaae9210350"), null, "18:2 Linoleic acid", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit linoleic (Omega-6)" },
                    { new Guid("0f2a40ec-7399-2f0b-5987-9b459fff20be"), null, "Fructose", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Đường fructose" },
                    { new Guid("105a2684-c8b2-3f06-51b4-df96bb295f29"), null, "Vitamin D IU", new Guid("a06cedbb-6209-6b82-bc1f-ca9873f9e31c"), "Vitamin D (IU)" },
                    { new Guid("21414437-603f-2de5-60fe-d0278e0872b5"), "Hàm lượng cồn trong thực phẩm/đồ uống.", "Alcohol, ethyl", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Cồn ethanol" },
                    { new Guid("21bb1f9d-d428-504a-e7c3-8419a1cedd23"), null, "Stigmasterol", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Stigmasterol" },
                    { new Guid("2526fbcd-fcb9-6660-93ce-a5a34ea61630"), null, "Methionine", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit amin Methionine" },
                    { new Guid("25a764cc-7b22-12b3-0245-158fb4f02198"), null, "Theobromine", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Theobromine" },
                    { new Guid("26651bb1-7bfe-d7df-dd2d-08f96eea1feb"), null, "Serine", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit amin Serine" },
                    { new Guid("27c4e3a0-6b2c-edde-9815-7bbc4cae355c"), null, "Campesterol", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Campesterol" },
                    { new Guid("2b9bb05a-d554-ed46-ebb1-781b7b67e2b6"), null, "Tryptophan", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit amin Tryptophan" },
                    { new Guid("318fc25c-3e11-cdc7-7e54-6b836ccae2b9"), "Tổng năng lượng của thực phẩm (kcal).", "Energy", new Guid("93d2464a-59b3-7951-21fe-5dc36fe13c45"), "Năng lượng" },
                    { new Guid("31902ac1-9002-2055-5dc7-ba0b7eeaa6e3"), null, "Maltose", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Đường maltose" },
                    { new Guid("3398fcc6-5ddc-e1b2-2e1a-cd7b81ab6aa9"), null, "Aspartic acid", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit aspartic" },
                    { new Guid("3ddb50a7-302d-dbdb-e545-e67ac4054db1"), null, "Beta-sitosterol", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Beta-sitosterol" },
                    { new Guid("3e0b8215-4bf0-3937-fd01-06fe1f3a3c6b"), null, "Carotene, alpha", new Guid("c75b9051-0442-cf83-bf9d-c7c30b1413ad"), "Alpha-carotene" },
                    { new Guid("3f5f3bde-4d3e-cbcc-9bdf-618270327bf0"), null, "Eicosapentaenoic acid (EPA)", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "EPA" },
                    { new Guid("3fde1b26-768f-765a-feda-3cec72679fdd"), null, "Glutamic acid", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit glutamic" },
                    { new Guid("5062ea22-0adb-2e25-f6e1-b690f3a15f75"), null, "Glycine", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit amin Glycine" },
                    { new Guid("5bfdc079-8860-db6b-83cd-fefb53c84b22"), null, "Leucine", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit amin Leucine" },
                    { new Guid("6b6110d8-655d-df7c-486e-e85acf7d2540"), null, "Pantothenic acid (B5)", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Vitamin B5" },
                    { new Guid("6c11a4e9-a2f7-da27-8710-2a9c8dd6da3b"), null, "Galactose", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Đường galactose" },
                    { new Guid("769ddb05-71ee-eefa-18e3-7a7d3f49c35e"), null, "Vitamin A, IU", new Guid("a06cedbb-6209-6b82-bc1f-ca9873f9e31c"), "Vitamin A (IU)" },
                    { new Guid("7819d15f-492c-a68f-7c22-69890be07852"), null, "Lutein + zeaxanthin", new Guid("c75b9051-0442-cf83-bf9d-c7c30b1413ad"), "Lutein + zeaxanthin" },
                    { new Guid("80c1876f-f5be-ed28-5e72-b3735811c5f0"), null, "Tyrosine", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit amin Tyrosine" },
                    { new Guid("86b9cbd0-b193-3c94-1267-d1b2463b224e"), null, "18:1 Oleic acid", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit oleic" },
                    { new Guid("87db95cc-2e12-66ff-ff88-bb5c6b537518"), null, "Proline", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit amin Proline" },
                    { new Guid("87ddb238-fb89-28a9-d792-949edc7aaf2a"), null, "Valine", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit amin Valine" },
                    { new Guid("8f030538-baf9-c6a0-b958-e573b6cff744"), null, "Lactose", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Đường lactose" },
                    { new Guid("99046a8e-3163-4240-4534-89e0c0218492"), null, "Arginine", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit amin Arginine" },
                    { new Guid("9eb73af0-abf0-8520-d5eb-ae1abc716b9d"), null, "Histidine", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit amin Histidine" },
                    { new Guid("a8c1352c-f6a4-31ff-9b3d-f002c7af13f4"), null, "Glucose", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Đường glucose" },
                    { new Guid("a9145240-ccfe-3639-9726-c5fcc6a19807"), null, "Fatty acids, total trans", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Chất béo trans" },
                    { new Guid("ada3daf8-c279-a4b4-d753-0f799a88da35"), null, "Fatty acids, total saturated", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Chất béo bão hòa" },
                    { new Guid("afacbf3c-902e-cdbc-b28c-d0770f328557"), null, "Cystine", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit amin Cystine" },
                    { new Guid("b22e4026-ba11-b2de-3e99-056b11791bc8"), null, "Fatty acids, total polyunsaturated", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Chất béo không bão hòa đa" },
                    { new Guid("b9ccf55b-2977-8954-5983-01316c3379b6"), null, "Lysine", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit amin Lysine" },
                    { new Guid("be53a666-7e98-040e-4b91-7c2f71a2f546"), null, "Biotin", new Guid("c75b9051-0442-cf83-bf9d-c7c30b1413ad"), "Biotin" },
                    { new Guid("c6ea5065-0e4d-b669-076a-4cf97ee6bdd6"), null, "Lycopene", new Guid("c75b9051-0442-cf83-bf9d-c7c30b1413ad"), "Lycopene" },
                    { new Guid("c6f2916d-5c73-1ca9-c8e0-754d27c5132b"), null, "Alanine", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit amin Alanine" },
                    { new Guid("c9e832fd-06ae-d729-aaeb-e197c85bb761"), null, "Threonine", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit amin Threonine" }
                });

            migrationBuilder.InsertData(
                table: "Nutrients",
                columns: new[] { "Id", "Description", "IsMacroNutrient", "Name", "UnitId", "VietnameseName" },
                values: new object[] { new Guid("cf288aeb-68b0-ef71-e03d-d3ef34fda9ce"), "Nguồn năng lượng cô đặc, cần cho hấp thu vitamin tan trong dầu.", true, "Total lipid (fat)", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Tổng chất béo" });

            migrationBuilder.InsertData(
                table: "Nutrients",
                columns: new[] { "Id", "Description", "Name", "UnitId", "VietnameseName" },
                values: new object[,]
                {
                    { new Guid("d8dd11c7-30c4-4956-0723-b0c095818d78"), null, "Cryptoxanthin, beta", new Guid("c75b9051-0442-cf83-bf9d-c7c30b1413ad"), "Beta-Cryptoxanthin" },
                    { new Guid("dd3c5de2-7cf9-f476-2b25-6462fde1dad0"), null, "Choline", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Choline" },
                    { new Guid("e38369bb-2bf7-714c-8982-c4b8037ab867"), null, "18:3 Linolenic acid", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit alpha-linolenic (Omega-3)" },
                    { new Guid("e38ae717-e808-93e3-0305-49d77f0cb2bc"), null, "Caffeine", new Guid("813af784-5297-5aea-f247-99ee1ceb39b5"), "Caffeine" },
                    { new Guid("e748a00c-8f55-b10e-fcab-4b800639ab99"), null, "Docosahexaenoic acid (DHA)", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "DHA" },
                    { new Guid("f33dcae4-dce9-70c9-6feb-843630231ce7"), null, "Sucrose", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Đường saccarose" },
                    { new Guid("f566cf28-767f-5621-6f95-03980a2b638f"), null, "Isoleucine", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Axit amin Isoleucine" },
                    { new Guid("f84f6327-2b00-e712-5d98-e07ccd005d18"), "Hàm lượng nước trong thực phẩm.", "Water", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Nước" },
                    { new Guid("fa81a189-596c-6dff-7d3d-078e8fa086d4"), null, "Fatty acids, total monounsaturated", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Chất béo không bão hòa đơn" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("0138bdbe-6512-9ccc-a332-791054106ef5"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("075edf8a-e1ec-4275-1f20-4aed6eed19d4"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("08346647-35b8-ea15-1f06-fd61d09d88c4"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("0a259dfb-f18b-7a8f-2fec-33630aac3ea1"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("0df85a80-795d-fe5a-e1c4-ffaae9210350"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("0f2a40ec-7399-2f0b-5987-9b459fff20be"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("105a2684-c8b2-3f06-51b4-df96bb295f29"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("21414437-603f-2de5-60fe-d0278e0872b5"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("21bb1f9d-d428-504a-e7c3-8419a1cedd23"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("2526fbcd-fcb9-6660-93ce-a5a34ea61630"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("25a764cc-7b22-12b3-0245-158fb4f02198"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("26651bb1-7bfe-d7df-dd2d-08f96eea1feb"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("27c4e3a0-6b2c-edde-9815-7bbc4cae355c"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("2b9bb05a-d554-ed46-ebb1-781b7b67e2b6"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("318fc25c-3e11-cdc7-7e54-6b836ccae2b9"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("31902ac1-9002-2055-5dc7-ba0b7eeaa6e3"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("3398fcc6-5ddc-e1b2-2e1a-cd7b81ab6aa9"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("3ddb50a7-302d-dbdb-e545-e67ac4054db1"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("3e0b8215-4bf0-3937-fd01-06fe1f3a3c6b"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("3f5f3bde-4d3e-cbcc-9bdf-618270327bf0"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("3fde1b26-768f-765a-feda-3cec72679fdd"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("5062ea22-0adb-2e25-f6e1-b690f3a15f75"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("5bfdc079-8860-db6b-83cd-fefb53c84b22"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("6b6110d8-655d-df7c-486e-e85acf7d2540"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("6c11a4e9-a2f7-da27-8710-2a9c8dd6da3b"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("769ddb05-71ee-eefa-18e3-7a7d3f49c35e"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("7819d15f-492c-a68f-7c22-69890be07852"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("80c1876f-f5be-ed28-5e72-b3735811c5f0"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("86b9cbd0-b193-3c94-1267-d1b2463b224e"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("87db95cc-2e12-66ff-ff88-bb5c6b537518"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("87ddb238-fb89-28a9-d792-949edc7aaf2a"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("8f030538-baf9-c6a0-b958-e573b6cff744"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("99046a8e-3163-4240-4534-89e0c0218492"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("9eb73af0-abf0-8520-d5eb-ae1abc716b9d"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("a8c1352c-f6a4-31ff-9b3d-f002c7af13f4"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("a9145240-ccfe-3639-9726-c5fcc6a19807"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("ada3daf8-c279-a4b4-d753-0f799a88da35"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("afacbf3c-902e-cdbc-b28c-d0770f328557"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("b22e4026-ba11-b2de-3e99-056b11791bc8"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("b9ccf55b-2977-8954-5983-01316c3379b6"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("be53a666-7e98-040e-4b91-7c2f71a2f546"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c6ea5065-0e4d-b669-076a-4cf97ee6bdd6"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c6f2916d-5c73-1ca9-c8e0-754d27c5132b"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c9e832fd-06ae-d729-aaeb-e197c85bb761"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("cf288aeb-68b0-ef71-e03d-d3ef34fda9ce"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("d8dd11c7-30c4-4956-0723-b0c095818d78"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("dd3c5de2-7cf9-f476-2b25-6462fde1dad0"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("e38369bb-2bf7-714c-8982-c4b8037ab867"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("e38ae717-e808-93e3-0305-49d77f0cb2bc"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("e748a00c-8f55-b10e-fcab-4b800639ab99"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("f33dcae4-dce9-70c9-6feb-843630231ce7"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("f566cf28-767f-5621-6f95-03980a2b638f"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("f84f6327-2b00-e712-5d98-e07ccd005d18"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("fa81a189-596c-6dff-7d3d-078e8fa086d4"));

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("58c77fe0-a3ba-f1c2-0518-3e8a6cc02696"),
                column: "CreatedAtUTC",
                value: new DateTime(2025, 12, 2, 3, 13, 51, 393, DateTimeKind.Utc).AddTicks(5915));

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
                columns: new[] { "Description", "Name" },
                values: new object[] { "Chống oxy hóa, bảo vệ tế bào.", "Vitamin E" });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("422833be-5c43-e625-7a6a-6a74c32794a6"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Chuyển hóa năng lượng, hỗ trợ thần kinh.", "Vitamin B1 (Thiamin)" });

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
                column: "Description",
                value: "Giúp xây dựng cơ bắp và tế bào.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("55fb9dc4-6bf0-06b4-e2c5-ca786f557d38"),
                column: "Description",
                value: "Chống oxy hóa, tăng cường miễn dịch.");

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("67dc4e3a-958b-9f2b-ba70-d7e6690b8f2d"),
                column: "Description",
                value: "Duy trì cân bằng nước và nhịp tim.");

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
                columns: new[] { "Description", "Name" },
                values: new object[] { "Giúp đông máu và duy trì xương khỏe mạnh.", "Vitamin K" });

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
                columns: new[] { "Description", "Name" },
                values: new object[] { "Tốt cho da, mắt và hệ thần kinh.", "Vitamin B2 (Riboflavin)" });

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
                columns: new[] { "Description", "Name" },
                values: new object[] { "Giúp chuyển hóa năng lượng, bảo vệ tim mạch.", "Vitamin B3 (Niacin)" });

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
                columns: new[] { "Description", "Name", "UnitId" },
                values: new object[] { "Giúp hấp thu canxi, tốt cho xương.", "Vitamin D", new Guid("a06cedbb-6209-6b82-bc1f-ca9873f9e31c") });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("dbe42ec4-51b1-f98d-66fe-6fed6bdcad0a"),
                columns: new[] { "Description", "Name", "VietnameseName" },
                values: new object[] { "Tổng lượng đường tự nhiên và thêm vào.", "Sugars", "Đường" });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("e19cd21d-2c38-e38f-7c55-f643fd65daf9"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Hỗ trợ tiêu hóa và giảm cholesterol.", "Dietary Fiber" });

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
                columns: new[] { "Description", "Name", "VietnameseName" },
                values: new object[] { "Nguồn năng lượng chính của cơ thể.", "Carbohydrate", "Tinh bột" });

            migrationBuilder.UpdateData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("ff39565c-1b2f-7db1-4f47-7b9ca86221f6"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Quan trọng cho phụ nữ mang thai và tế bào mới.", "Folate (Folic Acid)" });

            migrationBuilder.InsertData(
                table: "Nutrients",
                columns: new[] { "Id", "Description", "Name", "UnitId", "VietnameseName" },
                values: new object[] { new Guid("5e08cf55-2b60-6f26-ef65-305553ffb09b"), "Giúp sáng mắt và tăng sức đề kháng.", "Vitamin A", new Guid("a06cedbb-6209-6b82-bc1f-ca9873f9e31c"), "Vitamin A" });

            migrationBuilder.InsertData(
                table: "Nutrients",
                columns: new[] { "Id", "Description", "IsMacroNutrient", "Name", "UnitId", "VietnameseName" },
                values: new object[] { new Guid("73cd094d-61aa-61ce-d021-9ffa9b9ebbad"), "Tổng lượng chất béo trong thực phẩm.", true, "Fat", new Guid("20a23d6e-c0bf-5383-c8c3-dd19682ccf68"), "Tổng chất béo" });
        }
    }
}
