using System.ComponentModel.DataAnnotations;
using SEP490_FTCDHMM_API.Api.Dtos.NutrientDtos.NutrientTarget;

namespace SEP490_FTCDHMM_API.Api.Dtos.CustomHealthGoalDtos
{
    public class UpdateCustomHealthGoalRequest
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên phải từ 3 đến 100 ký tự")]
        public string Name { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Thông tin chi tiết không vượt quá 500 ký tự ")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Dinh dưỡng không được để trống")]
        public List<NutrientTargetRequest> Targets { get; set; } = new();
    }
}
