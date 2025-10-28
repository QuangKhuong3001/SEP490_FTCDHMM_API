using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
    {
        public void Configure(EntityTypeBuilder<Ingredient> builder)
        {
            builder.ToTable("Ingredients");
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Name)
                .IsUnicode(true)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(i => i.LastUpdatedUtc)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(x => x.Calories)
                .HasColumnType("decimal(10,3)")
                .IsRequired();

            builder.Property(i => i.Description)
                .HasMaxLength(1000)
                .IsUnicode(true)
                .IsRequired(false);

            builder.HasOne(i => i.Image)
                .WithMany()
                .HasForeignKey(i => i.ImageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(i => i.Name).IsUnique();

            builder.Property(i => i.LastUpdatedUtc)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasMany(i => i.Categories)
                .WithMany(c => c.Ingredients)
                .UsingEntity(j => j.ToTable("IngredientCategoryLink"));
        }
    }
}
