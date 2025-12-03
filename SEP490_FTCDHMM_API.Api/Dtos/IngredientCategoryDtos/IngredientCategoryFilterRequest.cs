using SEP490_FTCDHMM_API.Api.Dtos.Common;

namespace SEP490_FTCDHMM_API.Api.Dtos.IngredientCategoryDtos
{
    public class IngredientCategoryFilterRequest
    {
        public string? Keyword { get; set; }
        public PaginationParams PaginationParams { get; set; } = new PaginationParams();
    }
}
