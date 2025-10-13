using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.NutrientDtos
{
    public class NutrientRequest
    {
        [Required(ErrorMessage = "Missing NutrientId")]
        public Guid NutrientId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Min must be greater than 0")]
        public decimal? Min { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Max must be greater than 0")]
        public decimal? Max { get; set; }

        [Required(ErrorMessage = "Missing Median.")]
        [Range(0, double.MaxValue, ErrorMessage = "Median must be greater than 0")]
        public decimal Median { get; set; }
    }
}
