using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class CommentMentionConfiguration : IEntityTypeConfiguration<CommentMention>
    {
        public void Configure(EntityTypeBuilder<CommentMention> builder)
        {
            builder.ToTable("CommentMentions");

            builder.HasKey(x => new { x.CommentId, x.MentionedUserId });

            builder.HasOne(x => x.Comment)
                .WithMany(c => c.Mentions)
                .HasForeignKey(x => x.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.MentionedUser)
                .WithMany()
                .HasForeignKey(x => x.MentionedUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.CommentId)
                .HasDatabaseName("IX_CommentMentions_CommentId");
        }
    }
}
