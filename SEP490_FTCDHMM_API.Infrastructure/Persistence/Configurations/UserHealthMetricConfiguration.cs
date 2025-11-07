using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

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

            builder.Property(u => u.RecordedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.User)
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
