namespace SEP490_FTCDHMM_API.Application.Dtos.RoleDtos
{
    public class PermissionToggleRequest
    {
        public Guid PermissionActionId { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
