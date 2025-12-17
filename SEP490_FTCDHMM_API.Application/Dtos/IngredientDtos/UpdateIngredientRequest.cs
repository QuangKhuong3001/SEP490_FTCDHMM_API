using Microsoft.AspNetCore.Http;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos;

namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos
{
    public class UpdateIngredientRequest
    {
        public DateTime LastUpdatedUtc { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public List<NutrientRequest> Nutrients { get; set; } = new();
        public List<Guid> IngredientCategoryIds { get; set; } = new();
    }
}
