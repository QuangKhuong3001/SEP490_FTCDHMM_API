using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftCookingStep;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftRecipeIngredient;

namespace SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos
{
    public class DraftRecipeRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public required string Difficulty { get; set; }
        public int CookTime { get; set; }
        public FileUploadModel? Image { get; set; }
        public int? Ration { get; set; }
        public List<Guid> LabelIds { get; set; } = new();
        public List<DraftRecipeIngredientRequest> Ingredients { get; set; } = new();
        public List<DraftCookingStepRequest> CookingSteps { get; set; } = new();
        public List<Guid> TaggedUserIds { get; set; } = new();
    }
}
