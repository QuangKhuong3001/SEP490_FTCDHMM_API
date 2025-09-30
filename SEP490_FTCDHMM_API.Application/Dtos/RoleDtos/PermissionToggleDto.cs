namespace SEP490_FTCDHMM_API.Application.Dtos.RoleDtos
{
    public class PermissionToggleDto
    {
        public Guid PermissionActionId { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
