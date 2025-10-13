using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class IngredientCategoryAssignmentConfiguration : IEntityTypeConfiguration<IngredientCategoryAssignment>
    {
        public void Configure(EntityTypeBuilder<IngredientCategoryAssignment> builder)
        {
            builder.ToTable("IngredientCategoryAssignments");

            builder.HasKey(x => new { x.IngredientId, x.CategoryId });

            builder.HasOne(x => x.Ingredient)
                   .WithMany(i => i.IngredientCategoryAssignments)
                   .HasForeignKey(x => x.IngredientId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Category)
                   .WithMany(c => c.IngredientCategoryAssignments)
                   .HasForeignKey(x => x.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
