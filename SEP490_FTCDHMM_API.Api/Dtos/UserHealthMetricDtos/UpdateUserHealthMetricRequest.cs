using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.UserHealthMetricDtos
{
    public class UpdateUserHealthMetricRequest
    {
        [Required(ErrorMessage = "Cân nặng không được để trống")]
        [Range(0.1, 300, ErrorMessage = "Cân nặng phải nằm trong khoảng từ 0.1 đến 300 kg")]
        public decimal WeightKg { get; set; }

        [Required(ErrorMessage = "Chiều cao không được để trống")]
        [Range(30, 250, ErrorMessage = "Chiều cao phải nằm trong khoảng từ 30 đến 250 cm")]
        public decimal HeightCm { get; set; }

        [Range(2, 70, ErrorMessage = "Tỷ lệ mỡ cơ thể phải nằm trong khoảng từ 2% đến 70%")]
        public decimal? BodyFatPercent { get; set; }

        [Range(10, 150, ErrorMessage = "Khối lượng cơ phải nằm trong khoảng từ 10 đến 150 kg")]
        public decimal? MuscleMassKg { get; set; }

        [StringLength(300, ErrorMessage = "Ghi chú không được vượt quá 300 ký tự")]
        public string? Notes { get; set; }
    }
}
