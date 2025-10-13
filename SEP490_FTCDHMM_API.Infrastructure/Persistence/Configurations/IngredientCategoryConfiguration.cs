using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class IngredientCategoryConfiguration : IEntityTypeConfiguration<IngredientCategory>
    {
        public void Configure(EntityTypeBuilder<IngredientCategory> builder)
        {
            builder.ToTable("IngredientCategories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .HasMaxLength(150)
                   .IsRequired()
                   .IsUnicode(true);

            builder.HasIndex(c => c.Name).IsUnique();
        }
    }
}
