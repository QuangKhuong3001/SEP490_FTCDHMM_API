using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class HealthGoalConfiguration : IEntityTypeConfiguration<HealthGoal>
    {
        public void Configure(EntityTypeBuilder<HealthGoal> builder)
        {
            builder.ToTable("HealthGoals");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.HasMany(x => x.Targets)
                .WithOne(x => x.HealthGoal)
                .HasForeignKey(x => x.HealthGoalId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
