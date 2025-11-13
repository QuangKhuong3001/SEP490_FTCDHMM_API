using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.CookingStepDtos
{
    public class CookingStepRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập bước hướng dẫn")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Hướng dẫn bước phải từ 1-1000 ký tự")]
        public required string Instruction { get; set; }

        public IFormFile? Image { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thứ tự bước")]
        [Range(1, int.MaxValue, ErrorMessage = "Thứ tự bước phải lớn hơn 0")]
        public required int StepOrder { get; set; }
    }
}
