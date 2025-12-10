using SEP490_FTCDHMM_API.Application.Dtos.IngredientCategoryDtos;

namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos
{
    public class IngredientResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Calories { get; set; }
        public List<IngredientCategoryResponse> CategoryNames { get; set; } = new();
        public DateTime LastUpdatedUtc { get; set; }
        public bool IsNew { get; set; }
        public string? ImageUrl { get; set; }
    }
}
