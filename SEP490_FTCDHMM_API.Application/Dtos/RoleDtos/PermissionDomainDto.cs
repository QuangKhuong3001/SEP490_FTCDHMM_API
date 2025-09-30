namespace SEP490_FTCDHMM_API.Application.Dtos.RoleDtos
{
    public class PermissionDomainDto
    {
        public string DomainName { get; set; } = string.Empty;
        public List<PermissionActionDto> Actions { get; set; } = new();
    }
}
