namespace SEP490_FTCDHMM_API.Application.Dtos.ReportDtos
{
    public class ReportResponse
    {
        public Guid Id { get; set; }
        public Guid TargetId { get; set; }
        public string TargetType { get; set; } = string.Empty;
        public string TargetName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ReporterId { get; set; }
        public string ReporterName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }
}
