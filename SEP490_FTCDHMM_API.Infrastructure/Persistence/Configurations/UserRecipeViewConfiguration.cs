using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class UserRecipeViewConfiguration : IEntityTypeConfiguration<UserRecipeView>
    {
        public void Configure(EntityTypeBuilder<UserRecipeView> builder)
        {
            builder.ToTable("UserRecipeViews");

            builder.HasKey(urv => new { urv.UserId, urv.RecipeId });

            builder.HasOne(urv => urv.User)
                   .WithMany(u => u.ViewedRecipes)
                   .HasForeignKey(urv => urv.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(urv => urv.Recipe)
                   .WithMany(r => r.Views)
                   .HasForeignKey(urv => urv.RecipeId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.Property(urv => urv.ViewedAtUtc)
                   .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
