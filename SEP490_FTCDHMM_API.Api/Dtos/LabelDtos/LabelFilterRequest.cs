using SEP490_FTCDHMM_API.Api.Dtos.Common;

namespace SEP490_FTCDHMM_API.Api.Dtos.LabelDtos
{
    public class LabelFilterRequest
    {
        public string? Keyword { get; set; }
        public PaginationParams PaginationParams { get; set; } = new PaginationParams();
    }

}
