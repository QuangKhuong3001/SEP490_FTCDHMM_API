using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Domain.Services
{
    public interface IHealthGoalEvaluator
    {
        double Evaluate(NutritionProfile profile, HealthGoal goal);
    }
}
