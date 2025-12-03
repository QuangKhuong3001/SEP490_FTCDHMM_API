namespace SEP490_FTCDHMM_API.Domain.Specifications
{
    public class RecipeBasicFilterSpec
    {
        public List<Guid> IncludeIngredientIds { get; set; } = new();
        public List<Guid> ExcludeIngredientIds { get; set; } = new();
        public List<Guid> IncludeLabelIds { get; set; } = new();
        public List<Guid> ExcludeLabelIds { get; set; } = new();

        public string? Difficulty { get; set; }
        public string? Keyword { get; set; }
        public int? Ration { get; set; }
        public decimal? MaxCookTime { get; set; }
    }
}
