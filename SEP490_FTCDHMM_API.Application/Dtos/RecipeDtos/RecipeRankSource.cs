namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos
{
    public sealed class RecipeRankSource
    {
        public Guid RecipeId { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public IReadOnlyList<Guid> IngredientIds { get; set; } = [];
    }
}
