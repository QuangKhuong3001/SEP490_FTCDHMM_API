using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> b)
    {
        b.HasKey(x => new { x.RecipeId, x.IngredientId });
        b.Property(x => x.QuantityGram).HasPrecision(18, 3);
    }
}
