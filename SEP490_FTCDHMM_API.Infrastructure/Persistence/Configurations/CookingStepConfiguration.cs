using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class CookingStepConfiguration : IEntityTypeConfiguration<CookingStep>
    {
        public void Configure(EntityTypeBuilder<CookingStep> builder)
        {
            builder.ToTable("CookingSteps");

            builder.HasKey(cs => cs.Id);

            builder.Property(cs => cs.Instruction)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(cs => cs.StepOrder)
                   .IsRequired();

            builder.HasOne(cs => cs.Image)
                   .WithMany()
                   .HasForeignKey(cs => cs.ImageId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(cs => cs.Recipe)
                .WithMany(r => r.CookingSteps)
                .HasForeignKey(cs => cs.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
