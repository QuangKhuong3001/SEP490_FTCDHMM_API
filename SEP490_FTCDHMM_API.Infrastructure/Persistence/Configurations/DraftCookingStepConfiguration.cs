using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

public class DraftCookingStepConfiguration : IEntityTypeConfiguration<DraftCookingStep>
{
    public void Configure(EntityTypeBuilder<DraftCookingStep> builder)
    {
        builder.ToTable("DraftCookingSteps");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StepOrder)
            .IsRequired();

        builder.Property(x => x.Instruction)
            .HasMaxLength(2000);

        builder.HasOne(x => x.DraftRecipe)
            .WithMany(x => x.DraftCookingSteps)
            .HasForeignKey(x => x.DraftRecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
