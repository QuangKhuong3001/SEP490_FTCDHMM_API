using System.ComponentModel.DataAnnotations;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.CookingStep.CookingStepImage;

namespace SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.CookingStep
{
    public class CookingStepRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập bước hướng dẫn")]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = "Hướng dẫn bước phải từ 1-2000 ký tự")]
        public string Instruction { get; set; } = string.Empty;

        public List<CookingStepImageRequest> Images { get; set; } = new();

        [Required(ErrorMessage = "Vui lòng nhập thứ tự bước")]
        [Range(1, int.MaxValue, ErrorMessage = "Thứ tự bước phải lớn hơn 0")]
        public int StepOrder { get; set; }
    }
}
