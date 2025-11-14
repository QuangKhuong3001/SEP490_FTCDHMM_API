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
                   .HasMaxLength(2000);

            builder.Property(cs => cs.StepOrder)
                   .IsRequired();

            builder.HasOne(cs => cs.Recipe)
                   .WithMany(r => r.CookingSteps)
                   .HasForeignKey(cs => cs.RecipeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(cs => cs.CookingStepImages)
                   .WithOne(si => si.CookingStep)
                   .HasForeignKey(si => si.CookingStepId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
