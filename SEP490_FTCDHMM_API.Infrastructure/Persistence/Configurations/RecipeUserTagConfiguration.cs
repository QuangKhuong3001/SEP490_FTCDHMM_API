using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class RecipeUserTagConfiguration : IEntityTypeConfiguration<RecipeUserTag>
    {
        public void Configure(EntityTypeBuilder<RecipeUserTag> builder)
        {
            builder.ToTable("RecipeUserTags");

            builder.HasKey(x => new { x.RecipeId, x.TaggedUserId });

            builder.HasOne(x => x.Recipe)
                   .WithMany(r => r.RecipeUserTags)
                   .HasForeignKey(x => x.RecipeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.TaggedUser)
                   .WithMany()
                   .HasForeignKey(x => x.TaggedUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
