namespace SEP490_FTCDHMM_API.Application.Dtos.RoleDtos
{
    public class RolePermissionSettingDto
    {
        public Guid RoleId { get; set; }
        public List<PermissionToggleDto> Permissions { get; set; } = new();
    }
}
