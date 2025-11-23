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

            builder.HasKey(x => x.UserId);

            builder.HasOne(x => x.User)
                .WithOne()
                .HasForeignKey<UserHealthGoal>(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.HealthGoal)
                .WithMany()
                .HasForeignKey(x => x.HealthGoalId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.CustomHealthGoal)
                .WithMany()
                .HasForeignKey(x => x.CustomHealthGoalId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.UserId).IsUnique();

            builder.Property(x => x.ExpiredAtUtc)
                .HasColumnType("date");
        }
    }
}
