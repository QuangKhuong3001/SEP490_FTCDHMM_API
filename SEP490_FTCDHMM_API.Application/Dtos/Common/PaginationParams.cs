namespace SEP490_FTCDHMM_API.Application.Dtos.Common
{
    public class PaginationParams
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
    }
}
