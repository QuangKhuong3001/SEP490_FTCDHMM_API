namespace SEP490_FTCDHMM_API.Api.Dtos.ReportDtos
{
    public class CreateReportRequest
    {
        public Guid TargetId { get; set; }
        public string TargetType { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
