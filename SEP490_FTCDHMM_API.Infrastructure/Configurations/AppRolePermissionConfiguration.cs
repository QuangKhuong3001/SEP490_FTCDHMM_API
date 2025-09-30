using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Infrastructure.Configurations
{
    internal class AppRolePermissionConfiguration : IEntityTypeConfiguration<AppRolePermission>
    {
        public void Configure(EntityTypeBuilder<AppRolePermission> builder)
        {
            builder.ToTable(nameof(AppRolePermission));

            builder.HasKey(rp => new { rp.RoleId, rp.PermissionActionId });

            builder.HasOne(rp => rp.Role)
                   .WithMany(r => r.RolePermissions)
                   .HasForeignKey(rp => rp.RoleId);

            builder.HasOne(rp => rp.PermissionAction)
                   .WithMany(p => p.RolePermissionActions)
                   .HasForeignKey(rp => rp.PermissionActionId);

            builder.Property(rp => rp.IsActive)
                   .HasDefaultValue(false);
        }
    }
}
