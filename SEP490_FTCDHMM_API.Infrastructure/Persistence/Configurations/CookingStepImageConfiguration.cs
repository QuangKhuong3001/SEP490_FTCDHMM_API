using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class CookingStepImageConfiguration : IEntityTypeConfiguration<CookingStepImage>
    {
        public void Configure(EntityTypeBuilder<CookingStepImage> builder)
        {
            builder.ToTable("CookingStepImages");

            builder.HasKey(si => si.Id);

            builder.Property(cs => cs.ImageOrder)
                .IsRequired();

            builder.HasOne(si => si.CookingStep)
                   .WithMany(cs => cs.CookingStepImages)
                   .HasForeignKey(si => si.CookingStepId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(si => si.Image)
                   .WithMany()
                   .HasForeignKey(si => si.ImageId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
