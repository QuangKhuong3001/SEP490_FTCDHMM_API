using SEP490_FTCDHMM_API.Application.Dtos.Common;

namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientCategoryDtos
{
    public class IngredientCategoryFilterRequest
    {
        public string? Keyword { get; set; }
        public PaginationParams PaginationParams { get; set; } = new PaginationParams();
    }
}
