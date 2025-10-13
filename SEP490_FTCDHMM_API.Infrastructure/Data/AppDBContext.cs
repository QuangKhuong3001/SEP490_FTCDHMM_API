using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Persistence.SeedData;

namespace SEP490_FTCDHMM_API.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<EmailOtp> EmailOtps { get; set; }
        public DbSet<PermissionAction> PermissionActions { get; set; }
        public DbSet<AppRolePermission> AppRolePermissions { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientCategory> IngredientCategories { get; set; }
        public DbSet<IngredientCategoryAssignment> IngredientCategoryAssignments { get; set; }
        public DbSet<IngredientNutrient> IngredientNutrients { get; set; }
        public DbSet<Nutrient> Nutrients { get; set; }
        public DbSet<NutrientUnit> NutrientUnits { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientCategory> IngredientCategories { get; set; }
        public DbSet<IngredientCategoryAssignment> IngredientCategoryAssignments { get; set; }
        public DbSet<IngredientNutrient> IngredientNutrients { get; set; }
        public DbSet<Nutrient> Nutrients { get; set; }
        public DbSet<NutrientUnit> NutrientUnits { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<IdentityUserRole<Guid>>();

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.SeedRoles();
            modelBuilder.SeedPermissions();
            modelBuilder.SeedRolePermissions();
            modelBuilder.SeedNutrientUnits();
            modelBuilder.SeedNutrients();
            modelBuilder.SeedIngredientCategories();
        }
    }
}
