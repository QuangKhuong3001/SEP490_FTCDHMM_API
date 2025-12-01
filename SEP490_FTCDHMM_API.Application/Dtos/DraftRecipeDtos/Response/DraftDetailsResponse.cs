using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftCookingStep;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftRecipeIngredient;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftUserTagged;
using SEP490_FTCDHMM_API.Application.Dtos.LabelDtos;

namespace SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.Response
{
    public class DraftDetailsResponse
    {
        public required string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public required string Difficulty { get; set; }
        public int CookTime { get; set; }
        public string? ImageUrl { get; set; }
        public int? Ration { get; set; }
        public List<LabelResponse> Labels { get; set; } = new();
        public List<DraftRecipeIngredientResponse> Ingredients { get; set; } = new();
        public List<DraftCookingStepResponse> CookingSteps { get; set; } = new();
        public List<DraftRecipeUserTaggedResponse> TaggedUser { get; set; } = new();
    }
}
