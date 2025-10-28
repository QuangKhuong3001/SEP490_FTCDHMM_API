using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class CustomHealthGoalTargetConfiguration : IEntityTypeConfiguration<CustomHealthGoalTarget>
    {
        public void Configure(EntityTypeBuilder<CustomHealthGoalTarget> builder)
        {
            builder.ToTable("CustomHealthGoalTargets");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.MinValue)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(x => x.MaxValue)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.HasOne(x => x.CustomHealthGoal)
                .WithMany(x => x.Targets)
                .HasForeignKey(x => x.CustomHealthGoalId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Nutrient)
                .WithMany()
                .HasForeignKey(x => x.NutrientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
