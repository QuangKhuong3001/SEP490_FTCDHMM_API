using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class UserHealthMetricConfiguration : IEntityTypeConfiguration<UserHealthMetric>
    {
        public void Configure(EntityTypeBuilder<UserHealthMetric> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.WeightKg).HasPrecision(5, 2);
            builder.Property(x => x.HeightCm).HasPrecision(5, 2);
            builder.Property(x => x.BMI).HasPrecision(5, 2);
            builder.Property(x => x.BMR).HasPrecision(6, 2);
            builder.Property(x => x.TDEE).HasPrecision(6, 2);
            builder.Property(x => x.BodyFatPercent).HasPrecision(5, 2);
            builder.Property(x => x.MuscleMassKg).HasPrecision(5, 2);

            builder.Property(u => u.ActivityLevel)
            .HasConversion(
                g => g.Value,
                v => ActivityLevel.From(v)
            )
            .HasDefaultValueSql("'MODERATE'");

            builder.Property(u => u.RecordedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.User)
                    .WithMany(u => u.HealthMetrics)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.UserId, x.RecordedAt })
                .HasDatabaseName("IX_UserHealthMetrics_User_Time");

        }
    }
}
