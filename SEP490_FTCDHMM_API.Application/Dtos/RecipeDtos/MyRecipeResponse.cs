using SEP490_FTCDHMM_API.Application.Dtos.LabelDtos;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos
{
    public class MyRecipeResponse
    {
        public Guid Id { get; set; }
        public required string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public required DifficultyValue Difficulty { get; set; }
        public decimal CookTime { get; set; }
        public required int Ration { get; set; }
        public string? ImageUrl { get; set; }
        public required List<LabelResponse> Labels { get; set; }
        public required List<RecipeIngredientResponse> Ingredients { get; set; }
    }
}
