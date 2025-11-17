namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.RecipeIngredient
{
    public class RecipeIngredientResponse
    {
        public Guid IngredientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal QuantityGram { get; set; }
    }
}
