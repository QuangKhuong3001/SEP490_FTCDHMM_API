namespace SEP490_FTCDHMM_API.Application.Dtos.ReportDtos
{
    public class ReportDetailListResponse
    {
        public Guid TargetId { get; set; }
        public string TargetType { get; set; } = null!;
        public string TargetName { get; set; } = null!;
        public List<ReportDetailItem> Reports { get; set; } = new();
    }

}
