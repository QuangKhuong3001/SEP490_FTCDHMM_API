using Microsoft.AspNetCore.Http;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos;

namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos
{
    public class CreateIngredientRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<Guid> IngredientCategoryIds { get; set; } = new();
        public IFormFile? Image { get; set; }
        public List<NutrientRequest> Nutrients { get; set; } = new();
    }
}
