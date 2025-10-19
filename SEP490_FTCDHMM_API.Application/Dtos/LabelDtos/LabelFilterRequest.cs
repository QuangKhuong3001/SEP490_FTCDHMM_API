using SEP490_FTCDHMM_API.Application.Dtos.Common;

namespace SEP490_FTCDHMM_API.Application.Dtos.LabelDtos
{
    public class LabelFilterRequest
    {
        public string? Keyword { get; set; }
        public required PaginationParams PaginationParams { get; set; }

    }

}
