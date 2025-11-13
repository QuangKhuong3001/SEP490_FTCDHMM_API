using System.ComponentModel.DataAnnotations;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientTargetDtos;

namespace SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos
{
    public class UpdateHealthGoalRequest
    {
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Tên mục tiêu phải từ 1 đến 255 ký tự")]
        public string? Name { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        [MinLength(1, ErrorMessage = "Vui lòng định nghĩa ít nhất một chỉ tiêu dinh dưỡng")]
        public List<NutrientTargetRequest>? Targets { get; set; }
    }
}
