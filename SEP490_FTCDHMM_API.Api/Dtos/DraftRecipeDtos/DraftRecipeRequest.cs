using System.ComponentModel.DataAnnotations;
using SEP490_FTCDHMM_API.Api.Dtos.DraftRecipeDtos.DraftCookingStep;
using SEP490_FTCDHMM_API.Api.Dtos.DraftRecipeDtos.DraftRecipeIngredient;

namespace SEP490_FTCDHMM_API.Api.Dtos.DraftRecipeDtos
{
    public class DraftRecipeRequest
    {
        [Required(ErrorMessage = "Tên không được để trống.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên phải từ 3 đến 100 ký tự")]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "Easy";
        public int CookTime { get; set; }
        public IFormFile? Image { get; set; }
        public string? ExistingMainImageUrl { get; set; }
        public int? Ration { get; set; }
        public List<Guid> LabelIds { get; set; } = new();
        public List<DraftRecipeIngredientRequest> Ingredients { get; set; } = new();
        public List<DraftCookingStepRequest> CookingSteps { get; set; } = new();
        public List<Guid> TaggedUserIds { get; set; } = new();
    }
}
