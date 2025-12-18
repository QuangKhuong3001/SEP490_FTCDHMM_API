using SEP490_FTCDHMM_API.Application.Dtos.KMeans;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Jobs.Interfaces.PreComputedInterfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Enum;

namespace SEP490_FTCDHMM_API.Application.Jobs.Implementations.PreComputedImplementations
{
    public class ClusterRecommendationPrecomputeJob : IClusterRecommendationPrecomputeJob
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IClusterRecipeScoringSystem _clusterScoring;
        private readonly ICacheService _cache;

        private const int pageSize = 10;

        public ClusterRecommendationPrecomputeJob(
            IRecipeRepository recipeRepository,
            IClusterRecipeScoringSystem clusterScoring,
            ICacheService cache)
        {
            _recipeRepository = recipeRepository;
            _clusterScoring = clusterScoring;
            _cache = cache;
        }

        public async Task ExecuteAsync()
        {
            await _cache.RemoveByPrefixAsync("recommend");

            var recipes = await _recipeRepository.GetActiveRecentRecipesAsync();
            if (!recipes.Any())
                return;

            var clusters = await _cache.SetMembersJsonAsync<ClusterProfile>("cluster:profiles");
            if (!clusters.Any())
                return;

            await Precompute(recipes, clusters, MealType.Breakfast, new TimeSpan(8, 0, 0));
            await Precompute(recipes, clusters, MealType.Lunch, new TimeSpan(13, 0, 0));
            await Precompute(recipes, clusters, MealType.Dinner, new TimeSpan(19, 0, 0));
        }

        private async Task Precompute(
            List<Recipe> recipes,
            List<ClusterProfile> clusters,
            MealType meal,
            TimeSpan time)
        {
            var mealKey = meal.ToString().ToLower();

            foreach (var cluster in clusters)
            {
                var ranked = recipes
                    .Select(r => new
                    {
                        Recipe = r,
                        Score = _clusterScoring.CalculateClusterScore(r, cluster, time)
                    })
                    .OrderByDescending(x => x.Score)
                    .ThenByDescending(x => x.Recipe.UpdatedAtUtc)
                    .Take(pageSize)
                    .Select(x => x.Recipe)
                    .ToList();

                await _cache.SetAsync(
                    $"recommend:cluster:{cluster.ClusterId}:meal:{mealKey}:page:1",
                    ranked,
                    TimeSpan.FromHours(24));
            }
        }
    }
}
