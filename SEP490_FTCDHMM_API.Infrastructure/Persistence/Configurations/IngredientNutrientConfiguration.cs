using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class IngredientNutrientConfiguration : IEntityTypeConfiguration<IngredientNutrient>
    {
        public void Configure(EntityTypeBuilder<IngredientNutrient> builder)
        {
            builder.ToTable("IngredientNutrients");

            builder.HasKey(x => new { x.IngredientId, x.NutrientId });

            builder.HasOne(x => x.Ingredient)
                   .WithMany(i => i.IngredientNutrients)
                   .HasForeignKey(x => x.IngredientId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Nutrient)
                   .WithMany(n => n.IngredientNutrients)
                   .HasForeignKey(x => x.NutrientId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Min)
                   .HasPrecision(10, 3);

            builder.Property(x => x.Max)
                   .HasPrecision(10, 3);

            builder.Property(x => x.Median)
                   .HasPrecision(10, 3)
                   .IsRequired();

            builder.HasIndex(x => x.IngredientId);
            builder.HasIndex(x => x.NutrientId);
        }
    }
}
