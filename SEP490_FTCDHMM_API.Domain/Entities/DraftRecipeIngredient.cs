namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class DraftRecipeIngredient
    {
        public Guid DraftRecipeId { get; set; }
        public DraftRecipe DraftRecipe { get; set; } = null!;

        public Guid IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = null!;

        public decimal QuantityGram { get; set; }
    }
}
