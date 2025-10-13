namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class PermissionAction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }

        public Guid PermissionDomainId { get; set; }
        public PermissionDomain PermissionDomain { get; set; } = null!;

        public ICollection<AppRolePermission> RolePermissionActions { get; set; } = new List<AppRolePermission>();
    }
}
