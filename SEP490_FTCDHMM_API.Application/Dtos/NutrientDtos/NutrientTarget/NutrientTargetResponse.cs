namespace SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget
{
    public class NutrientTargetResponse
    {
        public Guid NutrientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TargetType { get; set; } = "Absolute";
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal? MinEnergyPct { get; set; }
        public decimal? MaxEnergyPct { get; set; }
        public decimal Weight { get; set; } = 1m;
    }
}
