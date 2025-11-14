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
                .HasMaxLength(2000);

            builder.Property(r => r.CreatedAtUtc)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(r => r.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(x => x.CookTime)
                .IsRequired();

            builder.Property(r => r.Ration)
                .IsRequired();

            builder.Property(r => r.Rating)
               .IsRequired()
               .HasDefaultValue(0);

            builder.Property(r => r.Difficulty)
                .HasConversion(
                    v => v.Value,
                    v => new DifficultyValue(v))
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Calories)
                .HasColumnType("decimal(10,3)");

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
        }
    }
}
