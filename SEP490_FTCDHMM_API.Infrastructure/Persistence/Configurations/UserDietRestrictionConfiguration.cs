using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class UserDietRestrictionConfiguration : IEntityTypeConfiguration<UserDietRestriction>
    {
        public void Configure(EntityTypeBuilder<UserDietRestriction> builder)
        {
            builder.ToTable("UserIngredientRestrictions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                   .IsRequired();

            builder.Property(x => x.Notes)
                   .HasMaxLength(255);

            builder.Property(u => u.Type)
                .HasConversion(
                    g => g.Value,
                    v => RestrictionType.From(v)
                )
                .HasDefaultValueSql("'ALLERGY'");

            builder.HasOne(x => x.User)
                   .WithMany(u => u.DietRestrictions)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Ingredient)
                   .WithMany()
                   .HasForeignKey(x => x.IngredientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.IngredientCategory)
                   .WithMany()
                   .HasForeignKey(x => x.IngredientCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.UserId, x.ExpiredAtUtc })
                .HasDatabaseName("IX_DietRestrictions_User_Active");

            builder.HasIndex(x => x.IngredientId)
                .HasDatabaseName("IX_DietRestrictions_Ingredient");

            builder.HasIndex(x => x.IngredientCategoryId)
                .HasDatabaseName("IX_DietRestrictions_Category");

        }
    }
}
