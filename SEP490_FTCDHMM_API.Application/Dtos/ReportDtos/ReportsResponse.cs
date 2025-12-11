namespace SEP490_FTCDHMM_API.Application.Dtos.ReportDtos
{
    public class ReportsResponse
    {
        public string TargetType { get; set; } = "";
        public Guid TargetId { get; set; }
        public string TargetName { get; set; } = "";
        public string? TargetUserName { get; set; }
        public Guid? RecipeId { get; set; }
        public int Count { get; set; }
        public DateTime LatestReportAtUtc { get; set; }
    }

}
