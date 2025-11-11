using SEP490_FTCDHMM_API.Application.Dtos.CookingStepDtos;
using SEP490_FTCDHMM_API.Application.Dtos.LabelDtos;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos
{
    public class RecipeDetailsResponse
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public required UserInteractionResponse Author { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public required DifficultyValue Difficulty { get; set; }
        public decimal CookTime { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public required int Ration { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsFavorited { get; set; }
        public bool IsSaved { get; set; }
        public required List<CookingStepResponse> CookingSteps { get; set; }
        public required List<LabelResponse> Labels { get; set; }
        public required List<RecipeIngredientResponse> Ingredients { get; set; }
    }
}
