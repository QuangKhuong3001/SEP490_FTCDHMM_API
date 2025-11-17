namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.RecipeIngredient
{
    public class RecipeIngredientRequest
    {
        public Guid IngredientId { get; set; }
        public decimal QuantityGram { get; set; }
    }
}
