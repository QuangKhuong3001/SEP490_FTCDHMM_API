namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public class NutritionProfile
    {
        public decimal TotalCalories { get; set; }
        public Dictionary<string, decimal> Nutrients { get; init; } = new Dictionary<string, decimal>();
        public decimal Get(string name) => Nutrients.TryGetValue(name, out var v) ? v : 0;
    }
}
