using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Services;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class HealthGoalEvaluator : IHealthGoalEvaluator
    {
        public double Evaluate(NutritionProfile profile, HealthGoal goal)
        {
            if (goal.Targets.Count == 0) return 0;

            double scoreSum = 0;
            decimal totalKcal = profile.TotalCalories <= 0 ? 1 : profile.TotalCalories;

            foreach (var t in goal.Targets)
            {
                profile.Nutrients.TryGetValue(t.Nutrient.Name, out var actual);
                double sub = 0;

                if (t.TargetType == NutrientTargetType.Absolute)
                {
                    sub = ScoreInRange(actual, t.MinValue, t.MaxValue);
                }
                else
                {
                    var kcal = NutrientEnergyFactor.KcalPerGram(t.Nutrient.Name) * actual;
                    var pct = totalKcal == 0 ? 0 : kcal / totalKcal;
                    sub = ScoreInRange(pct, t.MinEnergyPct, t.MaxEnergyPct);
                }

                var w = (double)(t.Weight <= 0 ? 1 : t.Weight);
                scoreSum += sub * w;
            }

            var totalW = (double)goal.Targets.Sum(x => x.Weight <= 0 ? 1 : x.Weight);
            var normalized = scoreSum / (totalW == 0 ? 1 : totalW) * 100;
            return Math.Clamp(normalized, 0, 100);
        }

        private static double ScoreInRange(decimal actual, decimal? min, decimal? max)
        {
            if (min.HasValue && max.HasValue && min.Value < max.Value)
            {
                if (actual < min.Value || actual > max.Value) return 0.3;

                return 1;
            }

            return 0.5;
        }
    }
}
