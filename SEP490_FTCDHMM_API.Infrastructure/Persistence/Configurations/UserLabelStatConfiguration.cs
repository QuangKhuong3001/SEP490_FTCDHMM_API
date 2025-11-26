namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using SEP490_FTCDHMM_API.Domain.Entities;

    public class UserLabelStatConfiguration : IEntityTypeConfiguration<UserLabelStat>
    {
        public void Configure(EntityTypeBuilder<UserLabelStat> builder)
        {
            builder.ToTable("UserLabelStats");

            builder.HasKey(x => new { x.UserId, x.LabelId });

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Label)
                .WithMany()
                .HasForeignKey(x => x.LabelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
