using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.RecipeIngredient;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos
{
    public class CopyRecipeRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public int CookTime { get; set; }
        public FileUploadModel? Image { get; set; }
        public int Ration { get; set; }
        public List<Guid> LabelIds { get; set; } = new();
        public List<RecipeIngredientRequest> Ingredients { get; set; } = new();
        public List<CookingStepRequest> CookingSteps { get; set; } = new();
        public List<Guid> TaggedUserIds { get; set; } = new();
    }
}
