using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.NutrientTargetDtos
{
    public class NutrientTargetRequest
    {
        [Required(ErrorMessage = "NutrientId is required.")]
        public Guid NutrientId { get; set; }
        public string TargetType { get; set; } = "Absolute";
        public decimal? MinValue { get; set; }
        public decimal? MedianValue { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal? MinEnergyPct { get; set; }
        public decimal? MedianEnergyPct { get; set; }
        public decimal? MaxEnergyPct { get; set; }

        [Required(ErrorMessage = "Weight is required.")]
        public decimal Weight { get; set; } = 1m;
    }

}