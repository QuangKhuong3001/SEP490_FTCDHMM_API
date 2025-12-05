using Microsoft.AspNetCore.Http;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos;
using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos
{
    public class UpdateIngredientRequest
    {
        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public required List<NutrientRequest> Nutrients { get; set; }
        public required List<Guid> IngredientCategoryIds { get; set; }
    }
}
