namespace SEP490_FTCDHMM_API.Application.Dtos.RoleDtos
{
    public class RoleResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
    }
}
