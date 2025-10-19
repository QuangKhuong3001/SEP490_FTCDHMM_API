using SEP490_FTCDHMM_API.Api.Dtos.Common;

namespace SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos
{
    public class RecipeFilterRequest
    {
        public string? Keyword { get; set; }
        public string? SortBy { get; set; }
        public Guid? LabelId { get; set; }
        public required PaginationParams PaginationParams { get; set; }
    }
}
