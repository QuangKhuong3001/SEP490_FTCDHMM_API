namespace SEP490_FTCDHMM_API.Application.Dtos.ReportDtos
{
    public class ReportDetailItem
    {
        public Guid ReportId { get; set; }
        public string ReporterName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedAtUtc { get; set; }
    }
}
