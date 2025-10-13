namespace SEP490_FTCDHMM_API.Application.Dtos.RoleDtos
{
    public class PermissionDomainRequest
    {
        public string DomainName { get; set; } = string.Empty;
        public List<PermissionActionRequest> Actions { get; set; } = new();
    }
}
