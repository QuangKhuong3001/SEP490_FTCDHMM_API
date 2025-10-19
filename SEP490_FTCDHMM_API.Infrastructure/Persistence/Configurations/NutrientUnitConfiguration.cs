using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class NutrientUnitConfiguration : IEntityTypeConfiguration<NutrientUnit>
    {
        public void Configure(EntityTypeBuilder<NutrientUnit> builder)
        {
            builder.ToTable("NutrientUnits");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(u => u.Symbol)
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(u => u.Description)
                   .HasMaxLength(255);

            builder.HasMany(u => u.Nutrients)
                   .WithOne(n => n.Unit)
                   .HasForeignKey(n => n.UnitId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
