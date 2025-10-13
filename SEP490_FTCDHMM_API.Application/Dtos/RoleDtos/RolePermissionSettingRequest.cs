namespace SEP490_FTCDHMM_API.Application.Dtos.RoleDtos
{
    public class RolePermissionSettingRequest
    {
        public Guid RoleId { get; set; }
        public List<PermissionToggleRequest> Permissions { get; set; } = new();
    }
}
