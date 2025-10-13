namespace SEP490_FTCDHMM_API.Application.Dtos.RoleDtos
{
    public class PermissionActionRequest
    {
        public Guid ActionId { get; set; }
        public string ActionName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
