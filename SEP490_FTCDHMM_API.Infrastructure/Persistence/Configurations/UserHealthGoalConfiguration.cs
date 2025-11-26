using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class UserHealthGoalConfiguration : IEntityTypeConfiguration<UserHealthGoal>
    {
        public void Configure(EntityTypeBuilder<UserHealthGoal> builder)
        {
            builder.ToTable("UserHealthGoals");

            builder.HasKey(x => x.UserId);

            builder.HasOne(x => x.User)
                .WithOne(u => u.UserHealthGoal)
                .HasForeignKey<UserHealthGoal>(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(u => u.Type)
                .HasConversion(
                    g => g.Value,
                    v => HealthGoalType.From(v)
                )
                .HasDefaultValueSql("'CUSTOM'");

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
