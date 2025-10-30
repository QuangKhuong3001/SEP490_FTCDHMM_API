using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class HealthGoalConflictConfiguration : IEntityTypeConfiguration<HealthGoalConflict>
    {
        public void Configure(EntityTypeBuilder<HealthGoalConflict> builder)
        {
            builder.ToTable("HealthGoalConflicts");

            builder.HasKey(c => c.Id);

            builder.HasOne(c => c.HealthGoalA)
                   .WithMany()
                   .HasForeignKey(c => c.HealthGoalAId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.HealthGoalB)
                   .WithMany()
                   .HasForeignKey(c => c.HealthGoalBId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(c => new { c.HealthGoalAId, c.HealthGoalBId }).IsUnique();

        }
    }
}
