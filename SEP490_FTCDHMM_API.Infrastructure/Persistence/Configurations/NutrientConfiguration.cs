using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class NutrientConfiguration : IEntityTypeConfiguration<Nutrient>
    {
        public void Configure(EntityTypeBuilder<Nutrient> builder)
        {
            builder.ToTable("Nutrients");

            builder.HasKey(n => n.Id);

            builder.Property(e => e.IsMacroNutrient)
                .HasDefaultValue(false);

            builder.Property(n => n.Name)
                   .HasMaxLength(200)
                   .IsRequired();

        }
    }
}
