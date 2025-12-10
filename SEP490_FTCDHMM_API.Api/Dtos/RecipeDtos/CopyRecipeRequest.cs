using System.ComponentModel.DataAnnotations;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.CookingStep;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.RecipeIngredient;

namespace SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos
{
    public class CopyRecipeRequest
    {
        [Required(ErrorMessage = "Tên công thức không được để trống")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Tên công thức phải từ 1-200 ký tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Mô tả không được vượt quá 2000 ký tự")]
        public string? Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Độ khó không được để trống")]
        public string Difficulty { get; set; } = string.Empty;

        [Range(1, 1440, ErrorMessage = "Thời gian nấu phải từ 1-1440 phút")]
        public int CookTime { get; set; }
        public IFormFile? Image { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Khẩu phần ăn tối thiểu là 1")]
        public int Ration { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ít nhất một nhãn")]
        public List<Guid> LabelIds { get; set; } = new();

        [Required(ErrorMessage = "Công thức phải có ít nhất một nguyên liệu")]
        public List<RecipeIngredientRequest> Ingredients { get; set; } = new();

        [Required(ErrorMessage = "Công thức phải có ít nhất một bước")]
        public List<CookingStepRequest> CookingSteps { get; set; } = new();
        public List<Guid> TaggedUserIds { get; set; } = new();
    }
}
