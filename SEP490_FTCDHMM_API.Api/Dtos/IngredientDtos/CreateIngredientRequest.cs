using System.ComponentModel.DataAnnotations;
using SEP490_FTCDHMM_API.Api.Dtos.NutrientDtos;

namespace SEP490_FTCDHMM_API.Api.Dtos.IngredientDtos
{
    public class CreateIngredientRequest
    {
        [Required(ErrorMessage = "Missing Name")]
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Name must be less than 200 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Missing Ingredient Category")]
        public required List<Guid> IngredientCategoryIds { get; set; }

        [Required(ErrorMessage = "Missing Image")]
        public required IFormFile Image { get; set; }

        [Required(ErrorMessage = "Missing Nutrients")]
        public required List<NutrientRequest> Nutrients { get; set; }


    }
}
