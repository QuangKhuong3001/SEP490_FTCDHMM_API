using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos
{
    public class RecipeIngredientRequest
    {
        [Required(ErrorMessage = "Missing Ingredient")]
        public Guid IngredientId { get; set; }
        [Required(ErrorMessage = "Missing Quantity")]
        public decimal QuantityGram { get; set; }
    }
}
