namespace SEP490_FTCDHMM_API.Application.Dtos.NutrientTargetDtos
{
    public class NutrientTargetRequest
    {
        public Guid NutrientId { get; set; }
        public string TargetType { get; set; } = "Absolute";
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal? MinEnergyPct { get; set; }
        public decimal? MaxEnergyPct { get; set; }
        public decimal Weight { get; set; } = 1m;
    }

}
