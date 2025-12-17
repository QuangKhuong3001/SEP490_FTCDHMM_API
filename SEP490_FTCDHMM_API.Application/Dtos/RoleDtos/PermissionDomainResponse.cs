namespace SEP490_FTCDHMM_API.Application.Dtos.RoleDtos
{
    public class PermissionDomainResponse
    {
        public string DomainName { get; set; } = string.Empty;
        public List<PermissionActionResponse> Actions { get; set; } = new();
    }
}
