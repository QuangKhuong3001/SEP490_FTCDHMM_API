using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> builder)
        {
            builder.ToTable("Recipes");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.Description)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(r => r.CreatedAtUtc)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(r => r.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(x => x.CookTime)
                .IsRequired();

            builder.Property(r => r.Ration)
                .IsRequired();

            builder.Property(r => r.AvgRating)
               .IsRequired()
               .HasDefaultValue(0);

            builder.Property(r => r.ViewCount)
               .IsRequired()
               .HasDefaultValue(0);

            builder.Property(r => r.RatingCount)
               .IsRequired()
               .HasDefaultValue(0);

            builder.Property(u => u.Difficulty)
                .HasConversion(
                    g => g.Value,
                    v => DifficultyValue.From(v)
                )
                .HasDefaultValueSql("'MEDIUM'");

            builder.Property(x => x.Calories)
                .HasColumnType("decimal(10,3)")
                .IsRequired();

            builder.HasOne(r => r.Author)
                .WithMany()
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.Labels)
                .WithMany(l => l.Recipes)
                .UsingEntity(j => j.ToTable("RecipeLabels"));

            builder.HasMany(r => r.RecipeIngredients)
                   .WithOne(ri => ri.Recipe)
                   .HasForeignKey(ri => ri.RecipeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.CookingSteps)
                .WithOne(cs => cs.Recipe)
                .HasForeignKey(cs => cs.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Image)
                .WithOne()
                .HasForeignKey<Recipe>(r => r.ImageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.FavoritedBy)
                .WithOne(f => f.Recipe)
                .HasForeignKey(f => f.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.SavedBy)
                .WithOne(s => s.Recipe)
                .HasForeignKey(s => s.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Views)
                .WithOne(v => v.Recipe)
                .HasForeignKey(v => v.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Comments)
                .WithOne(c => c.Recipe)
                .HasForeignKey(c => c.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Ratings)
                .WithOne(r => r.Recipe)
                .HasForeignKey(r => r.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.RecipeUserTags)
                .WithOne(t => t.Recipe)
                .HasForeignKey(t => t.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.NutritionAggregates)
                .WithOne(n => n.Recipe)
                .HasForeignKey(n => n.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
