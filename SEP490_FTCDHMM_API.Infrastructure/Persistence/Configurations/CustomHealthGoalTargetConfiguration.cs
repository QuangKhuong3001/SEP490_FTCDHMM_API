using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class CustomHealthGoalTargetConfiguration : IEntityTypeConfiguration<CustomHealthGoalTarget>
    {
        public void Configure(EntityTypeBuilder<CustomHealthGoalTarget> builder)
        {
            builder.ToTable("CustomHealthGoalTargets");

            builder.HasKey(x => x.Id);
            builder.Property(e => e.TargetType)
            .HasConversion(
                p => p.Value,
                v => NutrientTargetType.From(v)
            )
            .IsRequired()
            .HasMaxLength(50);

            builder.Property(x => x.MinValue).HasPrecision(18, 4);
            builder.Property(x => x.MaxValue).HasPrecision(18, 4);

            builder.Property(x => x.MinEnergyPct).HasPrecision(6, 4);
            builder.Property(x => x.MaxEnergyPct).HasPrecision(6, 4);

            builder.Property(x => x.Weight).HasPrecision(9, 3);

            builder.HasOne(x => x.CustomHealthGoal).WithMany(g => g.Targets).HasForeignKey(x => x.CustomHealthGoalId);
            builder.HasOne(x => x.Nutrient).WithMany().HasForeignKey(x => x.NutrientId);
        }
    }
}
