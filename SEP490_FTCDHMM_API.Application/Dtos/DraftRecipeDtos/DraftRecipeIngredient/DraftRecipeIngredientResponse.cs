namespace SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftRecipeIngredient
{
    public class DraftRecipeIngredientResponse
    {
        public Guid IngredientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal QuantityGram { get; set; }
    }
}
