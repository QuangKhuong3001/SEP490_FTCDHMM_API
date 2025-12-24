namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public class MealNutritionState
    {
        public decimal Calories { get; }
        public IReadOnlyDictionary<Guid, decimal> Nutrients { get; }

        public MealNutritionState(decimal calories, IReadOnlyDictionary<Guid, decimal> nutrients)
        {
            Calories = calories;
            Nutrients = nutrients;
        }
    }
}
