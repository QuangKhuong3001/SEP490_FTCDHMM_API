using System.ComponentModel.DataAnnotations;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.CookingStep;

namespace SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos
{
    public class CreateRecipeRequest
    {
        [Required(ErrorMessage = "Missing Name")]
        public required string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Missing Difficulty")]
        public required string Difficulty { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Cook time must be greater than 0")]
        public int CookTime { get; set; }
        public IFormFile? Image { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Ration must be greater than 0")]
        public required int Ration { get; set; }

        [Required(ErrorMessage = "Missing LabelIds")]
        public required List<Guid> LabelIds { get; set; }

        [Required(ErrorMessage = "Missing Ingredients")]
        public List<RecipeIngredientRequest> Ingredients { get; set; } = new();

        [Required(ErrorMessage = "Missing CookingSteps")]
        public required List<CookingStepRequest> CookingSteps { get; set; }
        public List<Guid> TaggedUserIds { get; set; } = new();

    }
}
