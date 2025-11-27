using SEP490_FTCDHMM_API.Application.Dtos.IngredientCategoryDtos;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos;

namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos
{
    public class IngredientDetailsResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Calories { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public string? ImageUrl { get; set; }
        public List<IngredientCategoryResponse> Categories { get; set; } = new();
        public List<NutrientResponse> Nutrients { get; set; } = new();
    }
}
