using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class RatingConfiguration : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.ToTable("Ratings");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Score)
                   .IsRequired();

            builder.Property(r => r.CreatedAtUtc)
                   .IsRequired();

            builder.Property(r => r.Feedback)
                   .HasMaxLength(256)
                   .IsRequired();

            builder.HasIndex(r => new { r.UserId, r.RecipeId }).IsUnique();

            builder.HasOne(r => r.User)
                   .WithMany()
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Recipe)
                   .WithMany(rp => rp.Ratings)
                   .HasForeignKey(r => r.RecipeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
