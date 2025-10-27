using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class HealthGoalTargetConfiguration : IEntityTypeConfiguration<HealthGoalTarget>
    {
        public void Configure(EntityTypeBuilder<HealthGoalTarget> builder)
        {
            builder.ToTable("HealthGoalTargets");

            builder.HasKey(x => x.Id);
            builder.Property(e => e.TargetType)
            .HasConversion(
                p => p.Value,
                v => NutrientTargetType.From(v)
            )
            .IsRequired()
            .HasMaxLength(50);

            builder.Property(x => x.MinValue).HasPrecision(18, 4);
            builder.Property(x => x.MedianValue).HasPrecision(18, 4);
            builder.Property(x => x.MaxValue).HasPrecision(18, 4);

            builder.Property(x => x.MinEnergyPct).HasPrecision(6, 4);
            builder.Property(x => x.MedianEnergyPct).HasPrecision(6, 4);
            builder.Property(x => x.MaxEnergyPct).HasPrecision(6, 4);

            builder.Property(x => x.Weight).HasPrecision(9, 3);

            builder.HasOne(x => x.HealthGoal).WithMany(g => g.Targets).HasForeignKey(x => x.HealthGoalId);
            builder.HasOne(x => x.Nutrient).WithMany().HasForeignKey(x => x.NutrientId);
        }
    }
}
