using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.Configurations
{
    internal class PermissionConfiguration : IEntityTypeConfiguration<PermissionAction>
    {
        public void Configure(EntityTypeBuilder<PermissionAction> builder)
        {
            builder.ToTable("PermissionActions");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(100);
        }
    }
}
