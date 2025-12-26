namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public class MealGap
    {
        public decimal RemainingCalories { get; }
        public IReadOnlyList<NutrientGap> RemainingNutrients { get; }

        public MealGap(decimal remainingCalories, IReadOnlyList<NutrientGap> remainingNutrients)
        {
            RemainingCalories = remainingCalories;
            RemainingNutrients = remainingNutrients;
        }
    }
}
