using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.NutrientDtos
{
    public class NutrientRequest
    {
        [Required(ErrorMessage = "Nutrient không được để trống")]
        public Guid NutrientId { get; set; }

        [Required(ErrorMessage = "Missing Median.")]
        [Range(0, 9999999.999, ErrorMessage = "Giá trị phải từ 0 đến 9999999.999")]
        public decimal Median { get; set; }
    }
}
