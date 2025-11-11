using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    internal class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(r => r.Id)
               .ValueGeneratedOnAdd();

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.DateOfBirth)
               .IsRequired();

            builder.Property(u => u.Gender)
                .HasConversion(
                    g => g.Value,
                    v => Gender.From(v)
                )
                .HasDefaultValueSql("'OTHER'");

            builder.Property(u => u.CreatedAtUtc)
               .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.ActivityLevel)
                .HasConversion(
                    g => g.Value,
                    v => ActivityLevel.From(v)
                )
                .HasDefaultValueSql("'MODERATE'");

            builder.Property(u => u.UserName)
               .IsRequired()
               .HasMaxLength(256);

            builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(256);

            builder.Property(u => u.EmailConfirmed)
               .IsRequired()
               .HasDefaultValue(false);

            builder.HasOne(u => u.Role)
                   .WithMany()
                   .HasForeignKey(u => u.RoleId)
                   .IsRequired();

            builder.HasOne(u => u.Image)
                    .WithOne()
                    .HasForeignKey<AppUser>(u => u.ImageId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(u => u.PhoneNumberConfirmed);
            builder.Ignore(u => u.PhoneNumber);
            builder.Ignore(u => u.TwoFactorEnabled);

        }
    }
}