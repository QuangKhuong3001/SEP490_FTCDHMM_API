using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.SeedData
{
    public static class ModelBuilderExtensions
    {
        private static Guid DeterministicGuid(string input)
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return new Guid(hash);
        }

        public static void SeedRoles(this ModelBuilder builder)
        {
            var roleType = typeof(RoleValue);

            var roles = roleType
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(f => f.FieldType == typeof(RoleValue))
                .Select(f => (RoleValue)f.GetValue(null)!)
                .ToList();

            var roleEntities = roles.Select(r => new AppRole
            {
                Id = DeterministicGuid(r.Name),
                Name = r.Name,
                NormalizedName = r.Name.ToUpperInvariant(),
                IsActive = true
            }).ToArray();

            builder.Entity<AppRole>().HasData(roleEntities);
        }

        public static void SeedPermissions(this ModelBuilder builder)
        {
            var permissionType = typeof(PermissionValue);

            var permissionValues = permissionType
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(f => f.FieldType == typeof(PermissionValue))
                .Select(f => (PermissionValue)f.GetValue(null)!)
                .ToList();

            var domains = permissionValues.GroupBy(p => p.Domain).ToList();

            var domainEntities = new List<PermissionDomain>();
            var actionEntities = new List<PermissionAction>();

            foreach (var domainGroup in domains)
            {
                var domainId = DeterministicGuid(domainGroup.Key);

                domainEntities.Add(new PermissionDomain
                {
                    Id = domainId,
                    Name = domainGroup.Key
                });

                foreach (var perm in domainGroup)
                {
                    actionEntities.Add(new PermissionAction
                    {
                        Id = DeterministicGuid($"{perm.Domain}:{perm.Action}"), // ✅ duy nhất
                        Name = perm.Action,
                        PermissionDomainId = domainId
                    });
                }
            }

            builder.Entity<PermissionDomain>().HasData(domainEntities);
            builder.Entity<PermissionAction>().HasData(actionEntities);
        }

        public static void SeedRolePermissions(this ModelBuilder builder)
        {
            var roles = new[]
            {
                new { Id = DeterministicGuid(RoleValue.Admin.Name), Name = RoleValue.Admin.Name },
                new { Id = DeterministicGuid(RoleValue.Moderator.Name), Name = RoleValue.Moderator.Name },
                new { Id = DeterministicGuid(RoleValue.Customer.Name), Name = RoleValue.Customer.Name }
            };

            var permissions = typeof(PermissionValue)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(f => f.FieldType == typeof(PermissionValue))
                .Select(f => (PermissionValue)f.GetValue(null)!)
                .ToList();

            var rolePermissions = new List<AppRolePermission>();

            foreach (var role in roles)
            {
                foreach (var p in permissions)
                {
                    rolePermissions.Add(new AppRolePermission
                    {
                        RoleId = role.Id,
                        PermissionActionId = DeterministicGuid($"{p.Domain}:{p.Action}"),
                        IsActive = role.Name == RoleValue.Admin.Name
                    });
                }
            }

            builder.Entity<AppRolePermission>().HasData(rolePermissions);
        }

        public static void SeedNutrientUnits(this ModelBuilder builder)
        {
            builder.Entity<NutrientUnit>().HasData(
                new NutrientUnit
                {
                    Id = DeterministicGuid("Unit:Gram"),
                    Name = "Gram (Gam)",
                    Symbol = "g",
                    Description = "Đơn vị dùng cho các đại dưỡng chất như protein, chất béo, tinh bột."
                },
                new NutrientUnit
                {
                    Id = DeterministicGuid("Unit:Milligram"),
                    Name = "Milligram (Miligam)",
                    Symbol = "mg",
                    Description = "Đơn vị dùng cho khoáng chất và vitamin thông thường."
                },
                new NutrientUnit
                {
                    Id = DeterministicGuid("Unit:Microgram"),
                    Name = "Microgram (Micromet)",
                    Symbol = "µg",
                    Description = "Đơn vị dùng cho vitamin vi lượng như B12, K, Folate..."
                },
                new NutrientUnit
                {
                    Id = DeterministicGuid("Unit:Kilocalorie"),
                    Name = "Kilocalorie (Kcal)",
                    Symbol = "kcal",
                    Description = "Đơn vị năng lượng thường gọi là calo."
                },
                new NutrientUnit
                {
                    Id = DeterministicGuid("Unit:InternationalUnit"),
                    Name = "International Unit (Đơn vị quốc tế)",
                    Symbol = "IU",
                    Description = "Dùng cho hoạt tính của vitamin A, D, E, K."
                }
            );
        }

        public static void SeedNutrients(this ModelBuilder builder)
        {
            var g = DeterministicGuid("Unit:Gram");
            var mg = DeterministicGuid("Unit:Milligram");
            var µg = DeterministicGuid("Unit:Microgram");
            var kcal = DeterministicGuid("Unit:Kilocalorie");
            var IU = DeterministicGuid("Unit:InternationalUnit");

            builder.Entity<Nutrient>().HasData(
                new Nutrient { Id = DeterministicGuid("Calories"), Name = "Calories", VietnameseName = "Năng lượng", Description = "Tổng năng lượng cung cấp (Energy)", UnitId = kcal },
                new Nutrient { Id = DeterministicGuid("Protein"), Name = "Protein", VietnameseName = "Chất đạm", Description = "Giúp xây dựng cơ bắp và tế bào.", UnitId = g },
                new Nutrient { Id = DeterministicGuid("Fat"), Name = "Fat", VietnameseName = "Tổng chất béo", Description = "Tổng lượng chất béo trong thực phẩm.", UnitId = g },
                new Nutrient { Id = DeterministicGuid("Carbohydrate"), Name = "Carbohydrate", VietnameseName = "Tinh bột", Description = "Nguồn năng lượng chính của cơ thể.", UnitId = g },
                new Nutrient { Id = DeterministicGuid("Fiber"), Name = "Dietary Fiber", VietnameseName = "Chất xơ", Description = "Hỗ trợ tiêu hóa và giảm cholesterol.", UnitId = g },
                new Nutrient { Id = DeterministicGuid("Sugars"), Name = "Sugars", VietnameseName = "Đường", Description = "Tổng lượng đường tự nhiên và thêm vào.", UnitId = g },
                new Nutrient { Id = DeterministicGuid("Cholesterol"), Name = "Cholesterol", VietnameseName = "Cholesterol", Description = "Cholesterol trong thực phẩm.", UnitId = mg },
                new Nutrient { Id = DeterministicGuid("Sodium"), Name = "Sodium", VietnameseName = "Natri", Description = "Giúp điều hòa nước và áp suất máu.", UnitId = mg },
                new Nutrient { Id = DeterministicGuid("Calcium"), Name = "Calcium", VietnameseName = "Canxi", Description = "Cần thiết cho xương và răng chắc khỏe.", UnitId = mg },
                new Nutrient { Id = DeterministicGuid("Iron"), Name = "Iron", VietnameseName = "Sắt", Description = "Thành phần của huyết sắc tố (hemoglobin).", UnitId = mg },
                new Nutrient { Id = DeterministicGuid("Magnesium"), Name = "Magnesium", VietnameseName = "Magie", Description = "Quan trọng cho cơ và thần kinh.", UnitId = mg },
                new Nutrient { Id = DeterministicGuid("Phosphorus"), Name = "Phosphorus", VietnameseName = "Phốt pho", Description = "Giúp hình thành xương và răng.", UnitId = mg },
                new Nutrient { Id = DeterministicGuid("Potassium"), Name = "Potassium", VietnameseName = "Kali", Description = "Duy trì cân bằng nước và nhịp tim.", UnitId = mg },
                new Nutrient { Id = DeterministicGuid("Vitamin A"), Name = "Vitamin A", VietnameseName = "Vitamin A", Description = "Giúp sáng mắt và tăng sức đề kháng.", UnitId = IU },
                new Nutrient { Id = DeterministicGuid("Vitamin C"), Name = "Vitamin C", VietnameseName = "Vitamin C", Description = "Tăng cường miễn dịch, chống oxy hóa.", UnitId = mg },
                new Nutrient { Id = DeterministicGuid("Vitamin D"), Name = "Vitamin D", VietnameseName = "Vitamin D", Description = "Giúp hấp thu canxi, tốt cho xương.", UnitId = IU },
                new Nutrient { Id = DeterministicGuid("Vitamin E"), Name = "Vitamin E", VietnameseName = "Vitamin E", Description = "Chống oxy hóa, bảo vệ tế bào.", UnitId = mg },
                new Nutrient { Id = DeterministicGuid("Vitamin K"), Name = "Vitamin K", VietnameseName = "Vitamin K", Description = "Giúp đông máu và duy trì xương khỏe mạnh.", UnitId = µg },
                new Nutrient { Id = DeterministicGuid("Thiamin"), Name = "Vitamin B1 (Thiamin)", VietnameseName = "Vitamin B1", Description = "Chuyển hóa năng lượng, hỗ trợ thần kinh.", UnitId = mg },
                new Nutrient { Id = DeterministicGuid("Riboflavin"), Name = "Vitamin B2 (Riboflavin)", VietnameseName = "Vitamin B2", Description = "Tốt cho da, mắt và hệ thần kinh.", UnitId = mg },
                new Nutrient { Id = DeterministicGuid("Niacin"), Name = "Vitamin B3 (Niacin)", VietnameseName = "Vitamin B3", Description = "Giúp chuyển hóa năng lượng, bảo vệ tim mạch.", UnitId = mg },
                new Nutrient { Id = DeterministicGuid("Vitamin B6"), Name = "Vitamin B6", VietnameseName = "Vitamin B6", Description = "Giúp tổng hợp hồng cầu, duy trì trao đổi chất.", UnitId = mg },
                new Nutrient { Id = DeterministicGuid("Folate"), Name = "Folate (Folic Acid)", VietnameseName = "Axit folic", Description = "Quan trọng cho phụ nữ mang thai và tế bào mới.", UnitId = µg },
                new Nutrient { Id = DeterministicGuid("Vitamin B12"), Name = "Vitamin B12", VietnameseName = "Vitamin B12", Description = "Giúp tạo máu và duy trì hệ thần kinh.", UnitId = µg },
                new Nutrient { Id = DeterministicGuid("Zinc"), Name = "Zinc", VietnameseName = "Kẽm", Description = "Hỗ trợ miễn dịch, da, tóc và móng.", UnitId = mg },
                new Nutrient { Id = DeterministicGuid("Copper"), Name = "Copper", VietnameseName = "Đồng", Description = "Tham gia hình thành tế bào máu và enzyme.", UnitId = mg },
                new Nutrient { Id = DeterministicGuid("Manganese"), Name = "Manganese", VietnameseName = "Mangan", Description = "Cần cho xương và não.", UnitId = mg },
                new Nutrient { Id = DeterministicGuid("Selenium"), Name = "Selenium", VietnameseName = "Selen", Description = "Chống oxy hóa, tăng cường miễn dịch.", UnitId = µg }
            );
        }


        public static void SeedIngredientCategories(this ModelBuilder builder)
        {
            var categories = new List<IngredientCategory>
            {
                new() { Id = DeterministicGuid("Category:Vegetables"), Name = "Rau củ quả" },
                new() { Id = DeterministicGuid("Category:Seafood"), Name = "Hải sản" },
                new() { Id = DeterministicGuid("Category:Meat"), Name = "Thịt" },
                new() { Id = DeterministicGuid("Category:Eggs"), Name = "Trứng" },
                new() { Id = DeterministicGuid("Category:Grains"), Name = "Ngũ cốc" },
                new() { Id = DeterministicGuid("Category:Spices"), Name = "Gia vị" },
                new() { Id = DeterministicGuid("Category:Oils"), Name = "Dầu mỡ" },
                new() { Id = DeterministicGuid("Category:Beverages"), Name = "Đồ uống" },
                new() { Id = DeterministicGuid("Category:Sweets"), Name = "Đồ ngọt" },
                new() { Id = DeterministicGuid("Category:Canned"), Name = "Đồ hộp / chế biến sẵn" },
                new() { Id = DeterministicGuid("Category:Others"), Name = "Nguyên liệu khác" }
            };

            builder.Entity<IngredientCategory>().HasData(categories);
        }
    }
}
