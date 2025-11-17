using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

public class DraftRecipeUserTagConfiguration : IEntityTypeConfiguration<DraftRecipeUserTag>
{
    public void Configure(EntityTypeBuilder<DraftRecipeUserTag> builder)
    {
        builder.ToTable("DraftRecipeUserTags");

        builder.HasKey(x => new { x.DraftRecipeId, x.TaggedUserId });

        builder.HasOne(x => x.DraftRecipe)
            .WithMany(x => x.DraftRecipeUserTags)
            .HasForeignKey(x => x.DraftRecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.TaggedUser)
            .WithMany()
            .HasForeignKey(x => x.TaggedUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
