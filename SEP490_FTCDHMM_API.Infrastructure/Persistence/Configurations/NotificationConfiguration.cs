using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Message)
                   .HasMaxLength(500)
                   .IsRequired(false);

            builder.Property(n => n.Type)
                   .HasConversion(
                       p => p.Name,
                       v => NotificationType.From(v)
                   )
                   .IsRequired();

            builder.HasOne(n => n.Receiver)
                   .WithMany()
                   .HasForeignKey(n => n.ReceiverId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.Sender)
                   .WithMany()
                   .HasForeignKey(n => n.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
