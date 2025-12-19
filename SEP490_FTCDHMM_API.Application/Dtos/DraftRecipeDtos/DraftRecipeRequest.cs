using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftCookingStep;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftRecipeIngredient;

namespace SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos
{
    public class DraftRecipeRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public int CookTime { get; set; }
        public FileUploadModel? Image { get; set; }

        /// ID of existing main image to keep (used when updating draft without changing main image)
        public Guid? ExistingMainImageId { get; set; }

        public int? Ration { get; set; }
        public List<Guid> LabelIds { get; set; } = new();
        public List<DraftRecipeIngredientRequest> Ingredients { get; set; } = new();
        public List<DraftCookingStepRequest> CookingSteps { get; set; } = new();
        public List<Guid> TaggedUserIds { get; set; } = new();
    }
}
