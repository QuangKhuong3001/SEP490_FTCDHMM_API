using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class UserHealthGoalConfiguration : IEntityTypeConfiguration<UserHealthGoal>
    {
        public void Configure(EntityTypeBuilder<UserHealthGoal> builder)
        {
            builder.ToTable("UserHealthGoals");

            builder.HasKey(x => new { x.UserId, x.HealthGoalId });

            builder.Property(x => x.CreatedAtUtc)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.User)
                .WithMany(u => u.HealthGoals)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.HealthGoal)
                .WithMany()
                .HasForeignKey(x => x.HealthGoalId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
