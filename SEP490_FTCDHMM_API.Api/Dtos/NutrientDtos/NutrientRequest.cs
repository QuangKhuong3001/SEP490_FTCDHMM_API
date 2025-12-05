using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.NutrientDtos
{
    public class NutrientRequest
    {
        public Guid NutrientId { get; set; }

        [Range(0, 9999999.999, ErrorMessage = "Giá trị Min phải nằm trong khoảng từ 0 đến 9999999.999")]
        public decimal? Min { get; set; }

        [Range(0, 9999999.999, ErrorMessage = "Giá trị Max phải nằm trong khoảng từ 0 đến 9999999.999")]
        public decimal? Max { get; set; }

        [Required(ErrorMessage = "Thiếu giá trị Median.")]
        [Range(0, 9999999.999, ErrorMessage = "Giá trị Median phải nằm trong khoảng từ 0 đến 9999999.999")]
        public decimal Median { get; set; }
    }
}
