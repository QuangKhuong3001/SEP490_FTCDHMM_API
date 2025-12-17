namespace SEP490_FTCDHMM_API.Application.Dtos.RoleDtos
{
    public class RoleDetailsResponse
    {
        public string Name { get; set; } = string.Empty;
        public DateTime LastUpdatedUtc { get; set; }
        public List<PermissionDomainResponse> Domains { get; set; } = new();

    }
}
