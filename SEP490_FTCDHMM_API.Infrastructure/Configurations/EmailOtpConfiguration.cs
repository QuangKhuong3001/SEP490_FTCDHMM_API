using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Configurations
{
    internal class EmailOtpConfiguration : IEntityTypeConfiguration<EmailOtp>
    {
        public void Configure(EntityTypeBuilder<EmailOtp> builder)
        {
            builder.ToTable(nameof(EmailOtp));

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(e => e.Purpose)
            .HasConversion(
                p => p.Value,
                v => OtpPurpose.From(v)
            )
            .IsRequired()
            .HasMaxLength(50);

            builder.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.ExpiresAtUtc)
                .IsRequired();

            builder.Property(e => e.Attempts)
                .HasDefaultValue(0);

            builder.Property(e => e.IsDisabled)
                .HasDefaultValue(false);

            builder.HasOne(e => e.User)
                   .WithMany()
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
