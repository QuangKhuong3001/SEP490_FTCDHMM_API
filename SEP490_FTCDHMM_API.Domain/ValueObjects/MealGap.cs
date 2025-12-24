namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public class MealGap
    {
        public decimal RemainingCalories { get; }
        public IReadOnlyDictionary<Guid, (decimal Min, decimal Max)> RemainingNutrients { get; }

        public MealGap(decimal remainingCalories, IReadOnlyDictionary<Guid, (decimal Min, decimal Max)> remainingNutrients)
        {
            RemainingCalories = remainingCalories;
            RemainingNutrients = remainingNutrients;
        }
    }
}
