using SEP490_FTCDHMM_API.Application.Dtos.LabelDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.RecipeIngredient;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response
{
    public class RecipeResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public UserInteractionResponse Author { get; set; } = new();
        public DifficultyValue Difficulty { get; set; } = DifficultyValue.Medium;
        public decimal CookTime { get; set; }
        public int Ration { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public List<LabelResponse> Labels { get; set; } = new();
        public List<RecipeIngredientResponse> Ingredients { get; set; } = new();
        public List<UserInteractionResponse> TaggedUser { get; set; } = new();
    }
}
