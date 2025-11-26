using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class CustomHealthGoalConfiguration : IEntityTypeConfiguration<CustomHealthGoal>
    {
        public void Configure(EntityTypeBuilder<CustomHealthGoal> builder)
        {
            builder.ToTable("CustomHealthGoals");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.Targets)
                .WithOne(x => x.CustomHealthGoal)
                .HasForeignKey(x => x.CustomHealthGoalId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
