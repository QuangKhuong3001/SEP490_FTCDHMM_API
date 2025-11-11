using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    internal class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.ToTable("Images");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Key)
                   .IsRequired();

            builder.Property(i => i.ContentType)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(i => i.CreatedAt)
                   .HasDefaultValueSql("GETDATE()");

            builder.HasOne(i => i.UploadedBy)
                   .WithMany()
                   .HasForeignKey(i => i.UploadedById)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
