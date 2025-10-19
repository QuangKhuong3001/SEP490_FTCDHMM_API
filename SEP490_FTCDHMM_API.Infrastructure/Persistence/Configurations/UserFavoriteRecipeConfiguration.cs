using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class UserFavoriteRecipeConfiguration : IEntityTypeConfiguration<UserFavoriteRecipe>
    {
        public void Configure(EntityTypeBuilder<UserFavoriteRecipe> builder)
        {
            builder.ToTable("UserFavoriteRecipes");

            builder.HasKey(f => new { f.UserId, f.RecipeId });

            builder.HasOne(f => f.User)
                   .WithMany(u => u.FavoriteRecipes)
                   .HasForeignKey(f => f.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(f => f.Recipe)
                   .WithMany(r => r.FavoritedBy)
                   .HasForeignKey(f => f.RecipeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(f => f.CreatedAtUtc).IsRequired();
        }
    }
}
