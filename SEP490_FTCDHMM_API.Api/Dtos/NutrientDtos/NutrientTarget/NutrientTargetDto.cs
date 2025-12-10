using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.NutrientDtos.NutrientTarget
{
    public class NutrientTargetDto
    {
        [Required(ErrorMessage = "Chưa chọn dinh dưỡng.")]
        public Guid NutrientId { get; set; }

        [Range(0, 9999, ErrorMessage = "Giá trị nhỏ nhất là 0.")]
        public decimal MinValue { get; set; }

        [Range(0, 9999, ErrorMessage = "Giá trị nhỏ nhất là 0.")]
        public decimal MaxValue { get; set; }
    }
}
