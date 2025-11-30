using SEP490_FTCDHMM_API.Application.Dtos.Common;

namespace SEP490_FTCDHMM_API.Application.Dtos.ReportDtos
{
    public class ReportFilterRequest
    {
        public PaginationParams PaginationParams { get; set; } = new PaginationParams();
        public string? Type { get; set; } = null;
        public string? Status { get; set; } = null;
        public string? Keyword { get; set; } = null;
    }
}
