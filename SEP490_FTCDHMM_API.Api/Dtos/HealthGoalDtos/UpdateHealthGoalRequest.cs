using System.ComponentModel.DataAnnotations;
using SEP490_FTCDHMM_API.Api.Dtos.NutrientDtos.NutrientTarget;

namespace SEP490_FTCDHMM_API.Api.Dtos.HealthGoalDtos
{
    public class UpdateHealthGoalRequest
    {
        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        [MinLength(1, ErrorMessage = "Vui lòng định nghĩa ít nhất một chỉ tiêu dinh dưỡng")]
        public List<NutrientTargetRequest> Targets { get; set; } = new();

        [Required(ErrorMessage = "Cần xác định thời gian cuối cùng chỉnh sửa mục tiêu.")]
        public DateTime? LastUpdatedUtc { get; set; }

    }
}
