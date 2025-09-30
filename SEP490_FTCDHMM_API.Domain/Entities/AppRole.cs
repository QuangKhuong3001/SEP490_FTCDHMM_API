using Microsoft.AspNetCore.Identity;

namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class AppRole : IdentityRole<Guid>
    {
        public bool IsActive { get; set; } = true;

        public ICollection<AppRolePermission> RolePermissions { get; set; } = new List<AppRolePermission>();
    }
}
