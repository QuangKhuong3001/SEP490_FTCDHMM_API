using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

public class DraftRecipeConfiguration : IEntityTypeConfiguration<DraftRecipe>
{
    public void Configure(EntityTypeBuilder<DraftRecipe> builder)
    {
        builder.ToTable("DraftRecipes");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.AuthorId)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(255);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.UpdatedAtUtc)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(r => r.Difficulty)
            .HasConversion(
                v => v.Value,
                v => new DifficultyValue(v))
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(x => x.Author)
            .WithOne()
            .HasForeignKey<DraftRecipe>(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Image)
            .WithMany()
            .HasForeignKey(x => x.ImageId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
