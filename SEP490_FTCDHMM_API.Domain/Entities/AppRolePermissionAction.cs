namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class AppRolePermission
    {
        public Guid RoleId { get; set; }
        public AppRole Role { get; set; } = null!;

        public Guid PermissionActionId { get; set; }
        public PermissionAction PermissionAction { get; set; } = null!;

        public bool IsActive { get; set; } = false;
    }
}
