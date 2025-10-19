using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class UserSaveRecipeConfiguration : IEntityTypeConfiguration<UserSaveRecipe>
    {
        public void Configure(EntityTypeBuilder<UserSaveRecipe> builder)
        {
            builder.ToTable("UserSaveRecipes");

            builder.HasKey(f => new { f.UserId, f.RecipeId });

            builder.HasOne(f => f.User)
                   .WithMany(u => u.SaveRecipes)
                   .HasForeignKey(f => f.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(f => f.Recipe)
                   .WithMany(r => r.SavedBy)
                   .HasForeignKey(f => f.RecipeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(f => f.CreatedAtUtc).IsRequired();
        }
    }
}
