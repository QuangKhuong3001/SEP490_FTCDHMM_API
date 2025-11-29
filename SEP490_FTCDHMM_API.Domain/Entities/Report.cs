using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class Report
    {
        public Guid Id { get; set; }
        public Guid ReporterId { get; set; }
        public AppUser Reporter { get; set; } = null!;
        public Guid TargetId { get; set; }
        public ReportObjectType TargetType { get; set; } = ReportObjectType.Recipe;
        public string Description { get; set; } = string.Empty;
        public ReportStatus Status { get; set; } = ReportStatus.Pending;
        public Guid? ReviewedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAtUtc { get; set; }
    }
}
