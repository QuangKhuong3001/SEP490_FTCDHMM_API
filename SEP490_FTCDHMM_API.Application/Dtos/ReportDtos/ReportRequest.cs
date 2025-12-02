namespace SEP490_FTCDHMM_API.Application.Dtos.ReportDtos
{
    public class ReportRequest
    {
        public Guid TargetId { get; set; }
        public string TargetType { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
