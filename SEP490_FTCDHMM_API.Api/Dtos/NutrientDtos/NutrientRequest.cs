using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.NutrientDtos
{
    public class NutrientRequest
    {
        [Required(ErrorMessage = "Missing NutrientId")]
        public Guid NutrientId { get; set; }

        [Range(0, 9999999.999, ErrorMessage = "Min must be between 0 and 9999999.999")]
        public decimal? Min { get; set; }

        [Range(0, 9999999.999, ErrorMessage = "Max must be between 0 and 9999999.999")]
        public decimal? Max { get; set; }

        [Required(ErrorMessage = "Missing Median.")]
        [Range(0, 9999999.999, ErrorMessage = "Median must be between 0 and 9999999.999")]
        public decimal Median { get; set; }
    }
}
