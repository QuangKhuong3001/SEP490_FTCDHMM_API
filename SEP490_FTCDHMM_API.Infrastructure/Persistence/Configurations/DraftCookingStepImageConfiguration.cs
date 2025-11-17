using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

public class DraftCookingStepImageConfiguration : IEntityTypeConfiguration<DraftCookingStepImage>
{
    public void Configure(EntityTypeBuilder<DraftCookingStepImage> builder)
    {
        builder.ToTable("DraftCookingStepImages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ImageOrder)
            .IsRequired();

        builder.HasOne(x => x.DraftCookingStep)
            .WithMany(x => x.DraftCookingStepImages)
            .HasForeignKey(x => x.DraftCookingStepId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Image)
            .WithMany()
            .HasForeignKey(x => x.ImageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
