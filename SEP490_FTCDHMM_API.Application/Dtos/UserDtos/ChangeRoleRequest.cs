namespace SEP490_FTCDHMM_API.Application.Dtos.UserDtos
{
    public class ChangeRoleRequest
    {
        public DateTime? LastUpdatedUtc { get; set; }
        public Guid RoleId { get; set; }
    }
}
