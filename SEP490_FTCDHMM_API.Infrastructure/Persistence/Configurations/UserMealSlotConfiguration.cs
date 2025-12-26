using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class UserMealSlotConfiguration : IEntityTypeConfiguration<UserMealSlot>
    {
        public void Configure(EntityTypeBuilder<UserMealSlot> builder)
        {
            builder.ToTable("UserMealSlots");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.EnergyPercent)
                .HasColumnType("decimal(5,2)");

            builder.HasOne(x => x.AppUser)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.UserId, x.OrderIndex })
                .IsUnique();
        }
    }
}
