namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class PermissionDomain
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public ICollection<PermissionAction> Actions { get; set; } = new List<PermissionAction>();
    }
}
