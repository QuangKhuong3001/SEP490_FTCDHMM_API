using SEP490_FTCDHMM_API.Application.Dtos.Common;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos
{
    public class RecipeFilterRequest
    {
        public string? Keyword { get; set; }
        public string? SortBy { get; set; }
        public string? Difficulty { get; set; }
        public int? Ration { get; set; }
        public decimal? MaxCookTime { get; set; }
        public List<Guid> LabelIds { get; set; } = new List<Guid>();
        public List<Guid> IngredientIds { get; set; } = new List<Guid>();
        public required PaginationParams PaginationParams { get; set; }
    }
}
