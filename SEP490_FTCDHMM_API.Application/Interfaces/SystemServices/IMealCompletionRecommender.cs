using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Interfaces.SystemServices
{
    public interface IMealCompletionRecommender
    {
        IEnumerable<(RecipeScoringSnapshot Recipe, double Score)> Recommend(
            RecommendationUserContext user,
            MealTarget target,
            MealNutritionState current,
            MealGap gap,
            IEnumerable<RecipeScoringSnapshot> candidates,
            ISet<Guid> excludedRecipeIds,
            int limit);
    }
}
