using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class HealthGoalCalculator : IHealthGoalCalculator
    {
        public HealthGoalCalculationResult Calculate(HealthGoalCalculationRequest request)
        {
            double bmr;
            Gender gender = Gender.From(request.Gender);

            if (gender.Equals(Gender.Male))
            {
                bmr = 10 * request.WeightKg + 6.25 * request.HeightCm - 5 * request.Age + 5;
            }
            else
            {
                bmr = 10 * request.WeightKg + 6.25 * request.HeightCm - 5 * request.Age - 161;
            }

            double tdee = bmr * request.ActivityLevel;

            double calorieAdjustment = request.GoalType.ToLower() switch
            {
                "weightloss" => -0.20,
                "musclegain" => +0.15,
                _ => 0
            };

            double targetCalories = tdee * (1 + calorieAdjustment);

            (double proteinPct, double fatPct, double carbPct) = request.GoalType.ToLower() switch
            {
                "weightloss" => (0.30, 0.25, 0.45),
                "musclegain" => (0.30, 0.20, 0.50),
                _ => (0.25, 0.25, 0.50)
            };

            decimal proteinGrams = (decimal)((targetCalories * proteinPct) / 4);
            decimal fatGrams = (decimal)((targetCalories * fatPct) / 9);
            decimal carbGrams = (decimal)((targetCalories * carbPct) / 4);

            return new HealthGoalCalculationResult
            {
                BMR = Math.Round(bmr, 2),
                TDEE = Math.Round(tdee, 2),
                TargetCalories = Math.Round(targetCalories, 2),
                ProteinGrams = Math.Round(proteinGrams, 2),
                FatGrams = Math.Round(fatGrams, 2),
                CarbGrams = Math.Round(carbGrams, 2),
                Summary = $"Goal: {request.GoalType} | Calories: {targetCalories:F0} kcal | Protein: {proteinGrams:F0}g | Fat: {fatGrams:F0}g | Carb: {carbGrams:F0}g"
            };
        }
    }
}
