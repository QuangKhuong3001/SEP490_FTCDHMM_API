using SEP490_FTCDHMM_API.Api.Dtos.DraftRecipeDtos.DraftCookingStep;
using SEP490_FTCDHMM_API.Api.Dtos.DraftRecipeDtos.DraftRecipeIngredient;

namespace SEP490_FTCDHMM_API.Api.Dtos.DraftRecipeDtos
{
    public class DraftRecipeRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "Easy";
        public int CookTime { get; set; }
        public IFormFile? Image { get; set; }
        public int? Ration { get; set; }
        public List<Guid> LabelIds { get; set; } = new();
        public List<DraftRecipeIngredientRequest> Ingredients { get; set; } = new();
        public List<DraftCookingStepRequest> CookingSteps { get; set; } = new();
        public List<Guid> TaggedUserIds { get; set; } = new();
    }
}
