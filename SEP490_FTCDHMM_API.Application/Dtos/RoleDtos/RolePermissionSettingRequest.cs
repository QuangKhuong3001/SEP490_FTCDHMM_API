namespace SEP490_FTCDHMM_API.Application.Dtos.RoleDtos
{
    public class RolePermissionSettingRequest
    {
        public List<PermissionToggleRequest> Permissions { get; set; } = new();
        public DateTime? LastUpdatedUtc { get; set; }
    }
}
