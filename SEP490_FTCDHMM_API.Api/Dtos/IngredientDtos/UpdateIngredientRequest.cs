using SEP490_FTCDHMM_API.Api.Dtos.NutrientDtos;

namespace SEP490_FTCDHMM_API.Api.Dtos.IngredientDtos
{
    public class UpdateIngredientRequest
    {
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public List<NutrientRequest>? Nutrients { get; set; }
    }
}
