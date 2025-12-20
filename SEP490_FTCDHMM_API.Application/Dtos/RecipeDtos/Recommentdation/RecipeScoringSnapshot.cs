namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation
{
    public sealed class RecipeScoringSnapshot
    {
        public Guid Id { get; init; }
        public DateTime UpdatedAtUtc { get; init; }
        public decimal Calories { get; set; }
        public int Ration { get; set; }
        public List<NutrientSnapshot> NutritionAggregates { get; init; } = [];
        public List<Guid> IngredientIds { get; init; } = [];
        public List<Guid> IngredientCategoryIds { get; init; } = [];
        public List<Guid> LabelIds { get; init; } = [];
    }

}
