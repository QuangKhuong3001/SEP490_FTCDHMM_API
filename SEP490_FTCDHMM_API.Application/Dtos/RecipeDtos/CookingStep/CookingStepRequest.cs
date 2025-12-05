using System.ComponentModel.DataAnnotations;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep.CookingStepImage;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep
{
    public class CookingStepRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập bước hướng dẫn")]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = "Hướng dẫn bước phải từ 1-2000 ký tự")]
        public required string Instruction { get; set; }
        public List<CookingStepImageRequest> Images { get; set; } = new();
        public required int StepOrder { get; set; }
    }
}
