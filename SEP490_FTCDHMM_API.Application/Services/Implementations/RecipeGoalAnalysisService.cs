using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Services;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

public class RecipeGoalAnalysisService : IRecipeGoalAnalysisService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IHealthGoalRepository _healthGoalRepository;
    private readonly IHealthGoalEvaluator _evaluator;

    public RecipeGoalAnalysisService(IRecipeRepository recipeRepository, IHealthGoalRepository healthGoalRepository, IHealthGoalEvaluator evaluator)
    {
        _recipeRepository = recipeRepository;
        _healthGoalRepository = healthGoalRepository;
        _evaluator = evaluator;
    }

    public async Task<HealthGoalAnalysisResponse> AnalyzeAsync(Guid recipeId, Guid goalId)
    {
        var recipe = await _recipeRepository.GetByIdAsync(recipeId, include: i => i.Include(r => r.NutritionAggregates).ThenInclude(n => n.Nutrient));

        var goal = await _healthGoalRepository.GetByIdAsync(goalId);

        if (recipe == null || goal == null)
            throw new AppException(AppResponseCode.NOT_FOUND);

        var profile = new NutritionProfile
        {
            TotalCalories = recipe.NutritionAggregates
                .Where(x => x.Nutrient.Name.Equals("Calories", StringComparison.OrdinalIgnoreCase))
                .Sum(x => x.Amount),
            Nutrients = recipe.NutritionAggregates
                .ToDictionary(x => x.Nutrient.Name, x => x.Amount)
        };

        var score = _evaluator.Evaluate(profile, goal);

        return new HealthGoalAnalysisResponse
        {
            RecipeId = recipe.Id,
            RecipeName = recipe.Name,
            HealthGoalId = goal.Id,
            HealthGoalName = goal.Name,
            MatchScore = score,
            TotalCalories = profile.TotalCalories,
            Nutrients = profile.Nutrients
        };
    }
}
