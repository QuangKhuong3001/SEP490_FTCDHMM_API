using Microsoft.AspNetCore.Http;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos;

namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos
{
    public class CreateIngredientRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required List<Guid> IngredientCategoryIds { get; set; }
        public required IFormFile Image { get; set; }
        public required List<NutrientRequest> Nutrients { get; set; }
    }
}
