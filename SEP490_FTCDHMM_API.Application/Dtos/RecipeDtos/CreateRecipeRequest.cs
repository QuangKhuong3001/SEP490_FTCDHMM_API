using Microsoft.AspNetCore.Http;
using SEP490_FTCDHMM_API.Application.Dtos.CookingStepDtos;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos
{
    public class CreateRecipeRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public required string Difficulty { get; set; }
        public double CookTime { get; set; }
        public IFormFile? Image { get; set; }
        public required int Ration { get; set; }
        public required List<Guid> LabelIds { get; set; }
        public required List<Guid> IngredientIds { get; set; }
        public required List<CookingStepRequest> CookingSteps { get; set; }
    }
}
