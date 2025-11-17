using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.NutrientDtos.NutrientTarget
{
    public class NutrientTargetDto
    {
        [Required(ErrorMessage = "NutrientId is required.")]
        public Guid NutrientId { get; set; }

        [Range(0, 9999, ErrorMessage = "MinValue must be greater than or equal to 0.")]
        public decimal MinValue { get; set; }

        [Range(0, 9999, ErrorMessage = "MaxValue must be greater than or equal to 0.")]
        public decimal MaxValue { get; set; }
    }
}
