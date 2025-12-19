using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces;
using SEP490_FTCDHMM_API.Domain.Enum;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation
{
    public class RecommendationService : IRecommentdationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IRecipeScoringSystem _recipeScoringSystem;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public RecommendationService(
            IUserRepository userRepository,
            IRecipeRepository recipeRepository,
            IMapper mapper,
            ICacheService cacheService,
            IRecipeScoringSystem recipeScoringSystem)
        {
            _userRepository = userRepository;
            _recipeRepository = recipeRepository;
            _mapper = mapper;
            _cacheService = cacheService;
            _recipeScoringSystem = recipeScoringSystem;
        }

        private MealType GetCurrentMeal()
        {
            var now = DateTime.Now.TimeOfDay;

            if (now >= new TimeSpan(5, 0, 0) && now < new TimeSpan(11, 0, 0))
                return MealType.Breakfast;

            if (now >= new TimeSpan(11, 0, 0) && now < new TimeSpan(16, 0, 0))
                return MealType.Lunch;

            return MealType.Dinner;
        }

        public async Task<PagedResult<RecipeRankResponse>> RecommendRecipesAsync(Guid userId, PaginationParams request)
        {
            if (request.PageNumber == 1)
            {
                var clusterId = await _cacheService.GetAsync<int>($"cluster:user:{userId}");
                var meal = GetCurrentMeal().ToString().ToLower();

                var key =
                    $"recommend:cluster:{clusterId}:meal:{meal}:page:1";

                var cached = await _cacheService.GetAsync<List<RecipeRankResponse>>(key);

                if (cached != null)
                {
                    for (var i = 0; i < cached.Count; i++)
                        cached[i].Score = null;

                    return new PagedResult<RecipeRankResponse>
                    {
                        Items = cached,
                        TotalCount = cached.Count,
                        PageNumber = 1,
                        PageSize = request.PageSize
                    };
                }
            }

            var user = await _userRepository.GetByIdAsync(
                id: userId,
                include: q =>
                    q.Include(u => u.UserHealthGoals)
                        .ThenInclude(uhg => uhg!.HealthGoal)
                            .ThenInclude(hg => hg!.Targets)
                                .ThenInclude(t => t.Nutrient)
                    .Include(u => u.UserHealthGoals)
                        .ThenInclude(uhg => uhg!.CustomHealthGoal)
                            .ThenInclude(chg => chg!.Targets)
                                .ThenInclude(t => t.Nutrient)
                    .Include(u => u.Ratings)
                    .Include(u => u.DietRestrictions)
                    .Include(u => u.HealthMetrics));

            var recipes = await _recipeRepository.GetActiveRecentRecipesAsync();

            if (!recipes.Any())
            {
                return new PagedResult<RecipeRankResponse>
                {
                    Items = new List<RecipeRankResponse>(),
                    TotalCount = 0,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }

            var scored = recipes
                .Select(r => (Recipe: r, Score: _recipeScoringSystem.CalculateFinalScore(user!, r)))
                .ToList();

            var ordered = scored
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Recipe.UpdatedAtUtc)
                .ToList();

            var totalCount = ordered.Count;

            var pagedTuples = ordered
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var pagedRecipes = pagedTuples.Select(x => x.Recipe).ToList();
            var mapped = _mapper.Map<List<RecipeRankResponse>>(pagedRecipes);

            for (var i = 0; i < mapped.Count; i++)
                mapped[i].Score = pagedTuples[i].Score;

            return new PagedResult<RecipeRankResponse>
            {
                Items = mapped,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}

