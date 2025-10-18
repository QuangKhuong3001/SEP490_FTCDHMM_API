using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;
namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class UserFollowConfiguration : IEntityTypeConfiguration<UserFollow>
    {
        public void Configure(EntityTypeBuilder<UserFollow> builder)
        {
            builder.ToTable("UserFollows");
            builder.HasKey(uf => new { uf.FollowerId, uf.FolloweeId });

            builder.HasOne(uf => uf.Follower)
                    .WithMany(u => u.Following)
                    .HasForeignKey(uf => uf.FollowerId)
                    .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(uf => uf.Followee)
                   .WithMany(u => u.Followers)
                   .HasForeignKey(uf => uf.FolloweeId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.Property(uf => uf.CreatedAtUtc)
                   .IsRequired();
        }
    }
}
