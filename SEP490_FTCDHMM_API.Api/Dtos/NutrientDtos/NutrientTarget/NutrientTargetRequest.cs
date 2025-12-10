using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.NutrientDtos.NutrientTarget
{
    public class NutrientTargetRequest
    {
        [Required(ErrorMessage = "Chưa chọn nutrient.")]
        public Guid NutrientId { get; set; }
        public string TargetType { get; set; } = "Absolute";

        [Range(0, double.MaxValue, ErrorMessage = "Giá trị phải lớn hơn 0")]
        public decimal? MinValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá trị phải lớn hơn 0")]
        public decimal? MaxValue { get; set; }

        [Range(0, 100, ErrorMessage = "Tỷ lệ năng lượng từ trong khoảng 0 đến 100%")]
        public decimal? MinEnergyPct { get; set; }

        [Range(0, 100, ErrorMessage = "Tỷ lệ năng lượng từ trong khoảng 0 đến 100%")]
        public decimal? MaxEnergyPct { get; set; }

        [Required(ErrorMessage = "Trọng số là bắt buộc .")]
        public decimal Weight { get; set; } = 1m;
    }

}