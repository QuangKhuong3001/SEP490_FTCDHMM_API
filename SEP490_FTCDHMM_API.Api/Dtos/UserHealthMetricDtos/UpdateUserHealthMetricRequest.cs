using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.UserHealthMetricDtos
{
    public class UpdateUserHealthMetricRequest
    {
        [Required(ErrorMessage = "Weight (kg) is required.")]
        [Range(0, 300, ErrorMessage = "Weight must be between 30kg and 300kg.")]
        public decimal WeightKg { get; set; }

        [Required(ErrorMessage = "Height (cm) is required.")]
        [Range(30, 250, ErrorMessage = "Height must be between 100cm and 250cm.")]
        public decimal HeightCm { get; set; }

        [Range(2, 70, ErrorMessage = "Body fat percentage must be between 2% and 70%.")]
        public decimal? BodyFatPercent { get; set; }

        [Range(10, 150, ErrorMessage = "Muscle mass must be between 10kg and 150kg.")]
        public decimal? MuscleMassKg { get; set; }

        [StringLength(300, ErrorMessage = "Notes cannot exceed 300 characters.")]
        public string? Notes { get; set; }
    }
}
