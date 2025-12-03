namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos
{
    public class RecipeFilterRequest
    {
        public string? Keyword { get; set; }
        public string? SortBy { get; set; }
        public string? Difficulty { get; set; }
        public int? Ration { get; set; }
        public decimal? MaxCookTime { get; set; }
        public List<Guid> IncludeLabelIds { get; set; } = new List<Guid>();
        public List<Guid> ExcludeLabelIds { get; set; } = new List<Guid>();
        public List<Guid> IncludeIngredientIds { get; set; } = new List<Guid>();
        public List<Guid> ExcludeIngredientIds { get; set; } = new List<Guid>();
        public RecipePaginationParams PaginationParams { get; set; } = new();
    }
}
