using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget
{
    public class NutrientTargetDto
    {
        public Guid NutrientId { get; set; }
        public NutrientTargetType TargetType { get; set; } = NutrientTargetType.Absolute;
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal? MinEnergyPct { get; set; }
        public decimal? MaxEnergyPct { get; set; }
        public int Weight { get; set; } = 1;
    }

}
