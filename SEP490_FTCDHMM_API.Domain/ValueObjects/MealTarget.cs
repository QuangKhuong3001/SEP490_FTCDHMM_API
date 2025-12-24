namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public class MealTarget
    {
        public decimal TargetCalories { get; }
        public IReadOnlyList<NutrientTarget> NutrientTargets { get; }

        public MealTarget(decimal targetCalories, IReadOnlyList<NutrientTarget> nutrientTargets)
        {
            TargetCalories = targetCalories;
            NutrientTargets = nutrientTargets;
        }
    }
}
