using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.CookingStepDtos;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos
{
    public class CreateRecipeRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public required string Difficulty { get; set; }
        public int CookTime { get; set; }
        public FileUploadModel? Image { get; set; }
        public required int Ration { get; set; }
        public required List<Guid> LabelIds { get; set; }
        public List<RecipeIngredientRequest> Ingredients { get; set; } = new();
        public required List<CookingStepRequest> CookingSteps { get; set; }
    }
}
