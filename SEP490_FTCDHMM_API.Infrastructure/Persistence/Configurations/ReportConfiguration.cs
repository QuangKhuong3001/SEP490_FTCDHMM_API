using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.ToTable("Reports");

            builder.HasKey(r => r.Id);

            builder.HasOne(r => r.Reporter)
                .WithMany()
                .HasForeignKey(r => r.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(r => r.TargetType)
                .HasConversion(
                    v => v.Value,
                    v => ReportObjectType.From(v))
                .HasMaxLength(30);

            builder.Property(r => r.Status)
                .HasConversion(
                    v => v.Value,
                    v => ReportStatus.From(v))
                .HasMaxLength(30);

            builder.Property(r => r.Description)
                .HasMaxLength(2000);
        }
    }
}
