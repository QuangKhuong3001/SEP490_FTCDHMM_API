namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public class NutrientTarget
    {
        public Guid NutrientId { get; }
        public NutrientTargetType TargetType { get; }
        public decimal MinValue { get; }
        public decimal MaxValue { get; }
        public decimal? MinEnergyPct { get; }
        public decimal? MaxEnergyPct { get; }
        public int Weight { get; }

        public NutrientTarget(
            Guid nutrientId,
            NutrientTargetType targetType,
            decimal minValue,
            decimal maxValue,
            decimal? minEnergyPct,
            decimal? maxEnergyPct,
            int weight)
        {
            NutrientId = nutrientId;
            TargetType = targetType;
            MinValue = minValue;
            MaxValue = maxValue;
            MinEnergyPct = minEnergyPct;
            MaxEnergyPct = maxEnergyPct;
            Weight = weight;
        }
    }
}
