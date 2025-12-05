using System.ComponentModel.DataAnnotations;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.RecipeIngredient;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos
{
    public class UpdateRecipeRequest
    {
        [Required(ErrorMessage = "Tên công thức không được để trống")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Tên công thức phải từ 1-200 ký tự")]
        public required string Name { get; set; }

        [StringLength(2000, ErrorMessage = "Mô tả không được vượt quá 2000 ký tự")]
        public string? Description { get; set; } = string.Empty;
        public required string Difficulty { get; set; }
        public required int CookTime { get; set; }
        public FileUploadModel? Image { get; set; }
        public required int Ration { get; set; }
        public required List<Guid> LabelIds { get; set; }
        public List<RecipeIngredientRequest> Ingredients { get; set; } = new();
        public required List<CookingStepRequest> CookingSteps { get; set; }
        public List<Guid> TaggedUserIds { get; set; } = new();
    }
}
