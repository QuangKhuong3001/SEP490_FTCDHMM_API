namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public class NutrientGap
    {
        public Guid NutrientId { get; }
        public decimal Min { get; }
        public decimal Max { get; }
        public double Weight { get; }

        public NutrientGap(
            Guid nutrientId,
            decimal min,
            decimal max,
            double weight)
        {
            NutrientId = nutrientId;
            Min = min;
            Max = max;
            Weight = weight;
        }
    }
}
