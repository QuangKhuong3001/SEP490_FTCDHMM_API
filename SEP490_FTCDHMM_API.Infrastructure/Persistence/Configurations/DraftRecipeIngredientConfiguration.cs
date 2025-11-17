using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

public class DraftRecipeIngredientConfiguration : IEntityTypeConfiguration<DraftRecipeIngredient>
{
    public void Configure(EntityTypeBuilder<DraftRecipeIngredient> builder)
    {
        builder.ToTable("DraftRecipeIngredients");

        builder.HasKey(x => new { x.DraftRecipeId, x.IngredientId });

        builder.Property(x => x.QuantityGram)
            .HasColumnType("decimal(10, 2)");

        builder.HasOne(x => x.DraftRecipe)
            .WithMany(x => x.DraftRecipeIngredients)
            .HasForeignKey(x => x.DraftRecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Ingredient)
            .WithMany()
            .HasForeignKey(x => x.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
