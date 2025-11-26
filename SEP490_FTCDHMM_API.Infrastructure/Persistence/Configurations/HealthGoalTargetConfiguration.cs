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

            builder.HasOne(x => x.HealthGoal)
                .WithMany(g => g.Targets)
                .HasForeignKey(x => x.HealthGoalId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.CustomHealthGoal)
                .WithMany(g => g.Targets)
                .HasForeignKey(x => x.CustomHealthGoalId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Nutrient)
                .WithMany()
                .HasForeignKey(x => x.NutrientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(u => u.TargetType)
               .HasConversion(
                   g => g.Value,
                   v => NutrientTargetType.From(v)
               )
               .HasDefaultValueSql("'ABSOLUTE'");

            builder.Property(x => x.MinValue)
                .HasPrecision(10, 2);

            builder.Property(x => x.MaxValue)
                .HasPrecision(10, 2);

            builder.Property(x => x.MinEnergyPct)
                .HasPrecision(5, 2);

            builder.Property(x => x.MaxEnergyPct)
                .HasPrecision(5, 2);

            builder.Property(x => x.Weight)
                .HasPrecision(6, 2)
                .IsRequired();
        }
    }
}
