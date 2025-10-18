using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Configurations;
using SEP490_FTCDHMM_API.Infrastructure.Persistence.SeedData;

namespace SEP490_FTCDHMM_API.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<EmailOtp> EmailOtps { get; set; }
        public DbSet<PermissionAction> Permissions { get; set; }
        public DbSet<AppRolePermission> AppRolePermissions { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientCategory> IngredientCategories { get; set; }
        public DbSet<IngredientCategoryAssignment> IngredientCategoryAssignments { get; set; }
        public DbSet<IngredientNutrient> IngredientNutrients { get; set; }
        public DbSet<Nutrient> Nutrients { get; set; }
        public DbSet<NutrientUnit> NutrientUnits { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<IdentityUserRole<Guid>>();

            modelBuilder.ApplyConfiguration(new AppUserConfiguration());
            modelBuilder.ApplyConfiguration(new AppRoleConfiguration());
            modelBuilder.ApplyConfiguration(new EmailOtpConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new AppRolePermissionConfiguration());
            modelBuilder.ApplyConfiguration(new ImageConfiguration());
            modelBuilder.ApplyConfiguration(new UserFollowConfiguration());
            modelBuilder.SeedRoles();
            modelBuilder.SeedPermissions();
            modelBuilder.SeedRolePermissions();

            modelBuilder.ApplyConfiguration(new UserFollowConfiguration());
        }
    }
}
