using System.ComponentModel.DataAnnotations;
using SEP490_FTCDHMM_API.Api.Dtos.NutrientDtos;

namespace SEP490_FTCDHMM_API.Api.Dtos.IngredientDtos
{
    public class UpdateIngredientRequest
    {
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }

        [Required(ErrorMessage = "Missing Required Nutrients")]
        public List<NutrientRequest>? Nutrients { get; set; }

        [Required(ErrorMessage = "Missing Ingredient Category")]
        public required List<Guid> IngredientCategoryIds { get; set; }
    }
}
