using SEP490_FTCDHMM_API.Api.Dtos.Common;

namespace SEP490_FTCDHMM_API.Api.Dtos.IngredientCategoryDtos
{
    public class IngredientCategoryFilterRequest
    {
        public string? Keyword { get; set; }
        public required PaginationParams PaginationParams { get; set; }
    }
}
