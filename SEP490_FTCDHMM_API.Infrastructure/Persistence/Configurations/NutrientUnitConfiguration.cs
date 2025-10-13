using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class NutrientUnitConfiguration : IEntityTypeConfiguration<NutrientUnit>
    {
        public void Configure(EntityTypeBuilder<NutrientUnit> builder)
        {
            builder.ToTable("NutrientUnits");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(u => u.Symbol)
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(u => u.Description)
                   .HasMaxLength(255);

            builder.HasMany(u => u.Nutrients)
                   .WithOne(n => n.Unit)
                   .HasForeignKey(n => n.UnitId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new NutrientUnit { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Gram", Symbol = "g", Description = "Used for macronutrients like protein, fat, carbs" },
                new NutrientUnit { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "Milligram", Symbol = "mg", Description = "Used for minerals and vitamins" },
                new NutrientUnit { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "Microgram", Symbol = "µg", Description = "Used for trace vitamins like B12" },
                new NutrientUnit { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Name = "Kilocalorie", Symbol = "kcal", Description = "Unit of energy (Calories)" },
                new NutrientUnit { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), Name = "Kilojoule", Symbol = "kJ", Description = "Alternative energy unit" },
                new NutrientUnit { Id = Guid.Parse("00000000-0000-0000-0000-000000000006"), Name = "Milliliter", Symbol = "mL", Description = "Used for liquid nutrients" },
                new NutrientUnit { Id = Guid.Parse("00000000-0000-0000-0000-000000000007"), Name = "Liter", Symbol = "L", Description = "Used for large liquid volumes" },
                new NutrientUnit { Id = Guid.Parse("00000000-0000-0000-0000-000000000008"), Name = "International Unit", Symbol = "IU", Description = "Used for vitamin activity (A, D, E, K)" },
                new NutrientUnit { Id = Guid.Parse("00000000-0000-0000-0000-000000000009"), Name = "Percent", Symbol = "%", Description = "Percentage of daily value" },
                new NutrientUnit { Id = Guid.Parse("00000000-0000-0000-0000-000000000010"), Name = "None", Symbol = "", Description = "No measurable unit" }
            );
        }
    }
}
