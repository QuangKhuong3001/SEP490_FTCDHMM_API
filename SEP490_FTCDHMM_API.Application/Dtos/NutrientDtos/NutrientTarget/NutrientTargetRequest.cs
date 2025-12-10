namespace SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget
{
    public class NutrientTargetRequest
    {
        public Guid NutrientId { get; set; }
        public string TargetType { get; set; } = "Absolute";
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal? MinEnergyPct { get; set; }
        public decimal? MaxEnergyPct { get; set; }
        public int Weight { get; set; } = 1;
    }

}
