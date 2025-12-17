using SEP490_FTCDHMM_API.Application.Dtos.LabelDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.RecipeIngredient;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response
{
    public class RecipeDetailsResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public UserInteractionResponse Author { get; set; } = new();
        public DateTime CreatedAtUtc { get; set; }
        public DifficultyValue Difficulty { get; set; } = DifficultyValue.Medium;
        public decimal CookTime { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public int Ration { get; set; }
        public int ViewCount { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsFavorited { get; set; }
        public bool IsSaved { get; set; }
        public List<CookingStepResponse> CookingSteps { get; set; } = new();
        public List<LabelResponse> Labels { get; set; } = new();
        public List<RecipeIngredientResponse> Ingredients { get; set; } = new();
        public List<UserInteractionResponse> TaggedUser { get; set; } = new();
        public RecipeParentResponse? Parent { get; set; }

    }
}
