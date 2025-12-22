using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.KMeans;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Jobs.Interfaces.PreComputedInterfaces;
using SEP490_FTCDHMM_API.Domain.Enum;

namespace SEP490_FTCDHMM_API.Application.Jobs.Implementations.PreComputedImplementations
{
    public class ClusterRecommendationPrecomputeJob : IClusterRecommendationPrecomputeJob
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IClusterRecipeScoringSystem _clusterScoring;
        private readonly ICacheService _cache;
        private readonly IMapper _mapper;

        private const int PageSize = 12;

        public ClusterRecommendationPrecomputeJob(
            IRecipeRepository recipeRepository,
            IClusterRecipeScoringSystem clusterScoring,
            IMapper mapper,
            ICacheService cache)
        {
            _recipeRepository = recipeRepository;
            _clusterScoring = clusterScoring;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task ExecuteAsync()
        {
            await _cache.RemoveByPrefixAsync("recommend");

            var snapshots = await _recipeRepository.GetRecipesForScoringAsync();
            if (!snapshots.Any())
                return;

            var clusters = await _cache.HashGetAllAsync<ClusterProfile>("cluster:profiles");
            if (!clusters.Any())
                return;

            await Precompute(snapshots, clusters, MealType.Breakfast, new TimeSpan(8, 0, 0));
            await Precompute(snapshots, clusters, MealType.Lunch, new TimeSpan(13, 0, 0));
            await Precompute(snapshots, clusters, MealType.Dinner, new TimeSpan(19, 0, 0));
        }

        private async Task Precompute(
            List<RecipeScoringSnapshot> recipes,
            List<ClusterProfile> clusters,
            MealType meal,
            TimeSpan time)
        {
            var mealKey = meal.ToString().ToLower();

            foreach (var cluster in clusters)
            {
                var scored = recipes
                    .Select(r => new
                    {
                        r.Id,
                        r.UpdatedAtUtc,
                        Score = _clusterScoring.CalculateClusterScore(r, cluster, time)
                    })
                    .OrderByDescending(x => x.Score)
                    .ThenByDescending(x => x.UpdatedAtUtc)
                    .Take(PageSize)
                    .ToList();

                var recipeIds = scored.Select(x => x.Id).ToList();

                var fullRecipes = await _recipeRepository.Query()
                    .AsNoTracking()
                    .Where(r => recipeIds.Contains(r.Id))
                    .Include(r => r.Author)
                        .ThenInclude(a => a.Avatar)
                    .Include(r => r.Image)
                    .Include(r => r.Labels)
                    .Include(r => r.NutritionAggregates)
                        .ThenInclude(na => na.Nutrient)
                    .Where(r => recipeIds.Contains(r.Id))
                    .ToListAsync();

                var map = fullRecipes.ToDictionary(r => r.Id);

                var ranked = _mapper.Map<List<RecipeRankResponse>>(
                    scored.Select(x => map[x.Id]).ToList()
                );

                for (var i = 0; i < ranked.Count; i++)
                    ranked[i].Score = scored[i].Score;

                await _cache.SetAsync(
                    $"recommend:cluster:{cluster.ClusterId}:meal:{mealKey}",
                    ranked,
                    TimeSpan.FromHours(24));
            }
        }
    }
}
