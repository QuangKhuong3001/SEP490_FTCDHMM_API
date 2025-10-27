using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

public class RecipeNutritionAggregateConfiguration : IEntityTypeConfiguration<RecipeNutritionAggregate>
{
    public void Configure(EntityTypeBuilder<RecipeNutritionAggregate> b)
    {
        b.HasKey(x => new { x.RecipeId, x.NutrientId });

        b.Property(x => x.Amount).HasPrecision(18, 4);
        b.Property(x => x.AmountPerServing).HasPrecision(18, 4);
        b.Property(x => x.ComputedAtUtc).HasDefaultValueSql("GETUTCDATE()");

        b.HasOne(x => x.Recipe)
            .WithMany(r => r.NutritionAggregates)
            .HasForeignKey(x => x.RecipeId);

        b.HasOne(x => x.Nutrient)
            .WithMany()
            .HasForeignKey(x => x.NutrientId);
    }
}
