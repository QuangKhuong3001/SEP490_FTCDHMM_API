using SEP490_FTCDHMM_API.Application.Dtos.IngredientCategoryDtos;

namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos
{
    public class IngredientResponse
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public List<IngredientCategoryResponse> CategoryNames { get; set; } = new();
        public DateTime LastUpdatedUtc { get; set; }
    }
}
