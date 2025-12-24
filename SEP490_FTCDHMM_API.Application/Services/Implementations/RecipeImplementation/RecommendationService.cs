using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Configurations;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.MealDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces;
using SEP490_FTCDHMM_API.Domain.Enum;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation
{
    public class RecommendationService : IRecommentdationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IRecipeScoringSystem _recipeScoringSystem;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IRatingRepository _ratingRepository;
        private readonly IUserRecipeViewRepository _userRecipeViewRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IUserSaveRecipeRepository _userSaveRecipeRepository;
        private readonly IMealTargetProvider _mealTargetProvider;
        private readonly IMealNutritionCalculator _mealNutritionCalculator;
        private readonly IMealGapCalculator _mealGapCalculator;
        private readonly IMealCompletionRecommender _mealCompletionRecommender;

        public RecommendationService(
            IUserRepository userRepository,
            IRecipeRepository recipeRepository,
            IMapper mapper,
            ICacheService cacheService,
            IRatingRepository ratingRepository,
            IUserRecipeViewRepository userRecipeViewRepository,
            ICommentRepository commentRepository,
            IUserSaveRecipeRepository userSaveRecipeRepository,
            IMealTargetProvider mealTargetProvider,
            IMealNutritionCalculator mealNutritionCalculator,
            IMealGapCalculator mealGapCalculator,
            IMealCompletionRecommender mealCompletionRecommender,
            IRecipeScoringSystem recipeScoringSystem)
        {
            _userRepository = userRepository;
            _recipeRepository = recipeRepository;
            _mapper = mapper;
            _cacheService = cacheService;
            _ratingRepository = ratingRepository;
            _userRecipeViewRepository = userRecipeViewRepository;
            _commentRepository = commentRepository;
            _userSaveRecipeRepository = userSaveRecipeRepository;
            _mealTargetProvider = mealTargetProvider;
            _mealNutritionCalculator = mealNutritionCalculator;
            _mealGapCalculator = mealGapCalculator;
            _mealCompletionRecommender = mealCompletionRecommender;
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

        public async Task<PagedResult<RecipeRankResponse>> ComputedRecommendRecipesAsync(Guid userId)
        {
            var meal = GetCurrentMeal().ToString().ToLower();
            var clusterId = await _cacheService.GetAsync<int?>($"cluster:user:{userId}");

            if (!clusterId.HasValue)
            {
                return new PagedResult<RecipeRankResponse>
                {
                    Items = new List<RecipeRankResponse>(),
                    TotalCount = 0,
                    PageNumber = 1,
                    PageSize = 0
                };
            }

            var cacheKey = $"recommend:cluster:{clusterId.Value}:meal:{meal}";
            var cached = await _cacheService.GetAsync<List<RecipeRankResponse>>(cacheKey);

            if (cached == null || cached.Count == 0)
            {
                return new PagedResult<RecipeRankResponse>
                {
                    Items = new List<RecipeRankResponse>(),
                    TotalCount = 0,
                    PageNumber = 1,
                    PageSize = 0
                };
            }

            var recipeIds = cached.Select(x => x.Id).ToList();

            var fullRecipes = await _recipeRepository.GetAllAsync(
                r => recipeIds.Contains(r.Id),
                q => q
                    .Include(r => r.Author)
                        .ThenInclude(a => a.Avatar)
                    .Include(r => r.Image)
                    .Include(r => r.Labels)
                    .Include(r => r.NutritionAggregates)
                        .ThenInclude(n => n.Nutrient)
            );

            var recipeMap = fullRecipes.ToDictionary(r => r.Id);

            var result = cached
                .Where(x => recipeMap.ContainsKey(x.Id))
                .Select(x =>
                {
                    var mapped = _mapper.Map<RecipeRankResponse>(recipeMap[x.Id]);
                    mapped.Score = null;
                    return mapped;
                })
                .ToList();

            return new PagedResult<RecipeRankResponse>
            {
                Items = result,
                TotalCount = result.Count,
                PageNumber = 1,
                PageSize = result.Count
            };
        }

        public async Task<PagedResult<RecipeRankResponse>> RecommendRecipesAsync(Guid userId, PaginationParams request)
        {
            var meal = GetCurrentMeal().ToString().ToLower();
            var cacheKey = $"recommend:user:{userId}:meal:{meal}";

            var cacheScored = await _cacheService.GetAsync<List<RankedRecipe>>(cacheKey);
            if (cacheScored != null)
            {
                var cacheRecipeIds = cacheScored.Select(x => x.Id).ToList();

                var fullCacheRecipes = await _recipeRepository.GetAllAsync(
                    r => cacheRecipeIds.Contains(r.Id),
                    q => q
                        .Include(r => r.Author).ThenInclude(a => a.Avatar)
                        .Include(r => r.Image)
                        .Include(r => r.Labels)
                        .Include(r => r.NutritionAggregates).ThenInclude(n => n.Nutrient)
                );

                var recipeCacheMap = fullCacheRecipes.ToDictionary(r => r.Id);

                var cacheOrdered = cacheScored
                    .Where(x => recipeCacheMap.ContainsKey(x.Id))
                    .Select(x => new { Recipe = recipeCacheMap[x.Id], x.Score })
                    .ToList();

                var cachePageItems = cacheOrdered
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var mapped = _mapper.Map<List<RecipeRankResponse>>(cachePageItems.Select(x => x.Recipe).ToList());

                for (int i = 0; i < mapped.Count; i++)
                    mapped[i].Score = cachePageItems[i].Score;

                return new PagedResult<RecipeRankResponse>
                {
                    Items = mapped,
                    TotalCount = cacheOrdered.Count,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }

            var snapshots = await _recipeRepository.GetRecipesForScoringAsync();
            if (!snapshots.Any())
            {
                return new PagedResult<RecipeRankResponse>
                {
                    Items = new List<RecipeRankResponse>(),
                    TotalCount = 0,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }

            var user = await _userRepository.GetByIdAsync(userId,
                include: i => i.Include(u => u.HealthMetrics));

            var tdee = user?.HealthMetrics
                .OrderByDescending(h => h.RecordedAt)
                .Select(h => (double?)h.TDEE)
                .FirstOrDefault();

            if (tdee == null || tdee <= 0)
            {
                return new PagedResult<RecipeRankResponse>
                {
                    Items = new List<RecipeRankResponse>(),
                    TotalCount = 0,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }

            var userWithGoals = (await _userRepository.GetAllAsync(
                u => u.Id == userId,
                q => q
                    .Include(u => u.UserHealthGoals)
                        .ThenInclude(g => g.HealthGoal)
                            .ThenInclude(h => h!.Targets)
                    .Include(u => u.UserHealthGoals)
                        .ThenInclude(g => g.CustomHealthGoal)
                            .ThenInclude(h => h!.Targets)
                    .Include(u => u.DietRestrictions)
            )).First();

            var now = DateTime.UtcNow;

            var domainTargets = userWithGoals.UserHealthGoals
                .Where(g =>
                    g.StartedAtUtc <= now &&
                    (g.ExpiredAtUtc == null || g.ExpiredAtUtc > now))
                .SelectMany(g => g.Type == HealthGoalType.SYSTEM
                    ? g.HealthGoal!.Targets
                    : g.CustomHealthGoal!.Targets)
                .Select(t => new NutrientTarget(
                    t.NutrientId,
                    t.TargetType,
                    t.MinValue ?? 0,
                    t.MaxValue ?? 0,
                    t.MinEnergyPct,
                    t.MaxEnergyPct,
                    t.Weight))
                .ToList();

            var restrictions = userWithGoals.DietRestrictions
                .Where(r => r.ExpiredAtUtc == null || r.ExpiredAtUtc > now)
                .ToList();

            var recipeLabelsMap = snapshots.ToDictionary(r => r.Id, r => r.LabelIds);

            var ratings = (await _ratingRepository.GetAllAsync(r => r.UserId == userId))
                .GroupBy(r => r.RecipeId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.CreatedAtUtc).First().Score);

            var comments = (await _commentRepository.GetAllAsync(c =>
                c.UserId == userId && c.CreatedAtUtc >= now.AddDays(-30)))
                .GroupBy(c => c.RecipeId)
                .ToDictionary(g => g.Key, g => g.Count());

            var views = (await _userRecipeViewRepository.GetAllAsync(v =>
                v.UserId == userId && v.ViewedAtUtc >= now.AddDays(-14)))
                .GroupBy(v => v.RecipeId)
                .ToDictionary(g => g.Key, g => g.Count());

            var savedIds = (await _userSaveRecipeRepository.GetAllAsync(s => s.UserId == userId))
                .Select(s => s.RecipeId)
                .ToHashSet();

            var userCtx = new RecommendationUserContext
            {
                UserId = userId,
                Tdee = tdee.Value,
                Targets = domainTargets,
                RestrictedIngredientIds = restrictions
                    .Where(r => r.IngredientId.HasValue)
                    .Select(r => r.IngredientId!.Value)
                    .ToHashSet(),
                RestrictedCategoryIds = restrictions
                    .Where(r => r.IngredientCategoryId.HasValue)
                    .Select(r => r.IngredientCategoryId!.Value)
                    .ToHashSet(),
                RatingByLabel = RecommendationBehaviorBuilder.BuildRatingByLabel(ratings, recipeLabelsMap),
                ViewByLabel = RecommendationBehaviorBuilder.BuildViewByLabel(views, recipeLabelsMap),
                CommentByLabel = RecommendationBehaviorBuilder.BuildCommentByLabel(comments, recipeLabelsMap),
                SaveByLabel = RecommendationBehaviorBuilder.BuildSaveByLabel(savedIds, recipeLabelsMap)
            };

            var scored = snapshots
                .Select(r => new RankedRecipe
                {
                    Id = r.Id,
                    UpdatedAtUtc = r.UpdatedAtUtc,
                    Score = _recipeScoringSystem.CalculateFinalScore(userCtx, r)
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.UpdatedAtUtc)
                .ToList();

            await _cacheService.SetAsync(cacheKey, scored, TimeSpan.FromHours(1));

            var recipeIds = scored.Select(x => x.Id).ToList();

            var fullRecipes = await _recipeRepository.GetAllAsync(
                r => recipeIds.Contains(r.Id),
                q => q
                    .Include(r => r.Author).ThenInclude(a => a.Avatar)
                    .Include(r => r.Image)
                    .Include(r => r.Labels)
                    .Include(r => r.NutritionAggregates).ThenInclude(n => n.Nutrient)
            );

            var recipeMap = fullRecipes.ToDictionary(r => r.Id);

            var ordered = scored
                .Where(x => recipeMap.ContainsKey(x.Id))
                .Select(x => new { Recipe = recipeMap[x.Id], x.Score })
                .ToList();

            var pageItems = ordered
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var mappedResult = _mapper.Map<List<RecipeRankResponse>>(pageItems.Select(x => x.Recipe).ToList());

            for (int i = 0; i < mappedResult.Count; i++)
                mappedResult[i].Score = pageItems[i].Score;

            return new PagedResult<RecipeRankResponse>
            {
                Items = mappedResult,
                TotalCount = ordered.Count,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<MealAnalyzeResponse> AnalyzeMealAsync(Guid userId, MealAnalyzeRequest request)
        {
            var mealType = GetCurrentMeal();
            var currentIds = request.CurrentRecipeIds?.Distinct().ToHashSet() ?? new HashSet<Guid>();

            var users = await _userRepository.GetAllAsync(u => u.Id == userId,
                include: i => i.Include(u => u.HealthMetrics));
            var user = users.FirstOrDefault();

            var tdee = user?.HealthMetrics
                .OrderByDescending(h => h.RecordedAt)
                .Select(h => (double?)h.TDEE)
                .FirstOrDefault();

            if (tdee == null || tdee <= 0)
            {
                return new MealAnalyzeResponse
                {
                    MealType = mealType,
                    TargetCalories = 0,
                    CurrentCalories = 0,
                    RemainingCalories = 0,
                    EnergyCoveragePercent = 0
                };
            }

            var userWithGoals = (await _userRepository.GetAllAsync(
                u => u.Id == userId,
                q => q
                    .Include(u => u.UserHealthGoals)
                        .ThenInclude(g => g.HealthGoal)
                            .ThenInclude(h => h!.Targets)
                    .Include(u => u.UserHealthGoals)
                        .ThenInclude(g => g.CustomHealthGoal)
                            .ThenInclude(h => h!.Targets)
                    .Include(u => u.DietRestrictions)
            )).First();

            var now = DateTime.UtcNow;

            var domainTargets = userWithGoals.UserHealthGoals
                .Where(g =>
                    g.StartedAtUtc <= now &&
                    (g.ExpiredAtUtc == null || g.ExpiredAtUtc > now))
                .SelectMany(g => g.Type == HealthGoalType.SYSTEM
                    ? g.HealthGoal!.Targets
                    : g.CustomHealthGoal!.Targets)
                .Select(t => new NutrientTarget(
                    t.NutrientId,
                    t.TargetType,
                    t.MinValue ?? 0,
                    t.MaxValue ?? 0,
                    t.MinEnergyPct,
                    t.MaxEnergyPct,
                    t.Weight))
                .ToList();

            var restrictions = userWithGoals.DietRestrictions
                .Where(r => r.ExpiredAtUtc == null || r.ExpiredAtUtc > now)
                .ToList();

            var snapshots = await _recipeRepository.GetRecipesForScoringAsync();
            var recipeLabelsMap = snapshots.ToDictionary(r => r.Id, r => r.LabelIds);

            var ratings = (await _ratingRepository.GetAllAsync(r => r.UserId == userId))
                .GroupBy(r => r.RecipeId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.CreatedAtUtc).First().Score);

            var comments = (await _commentRepository.GetAllAsync(c =>
                c.UserId == userId && c.CreatedAtUtc >= now.AddDays(-30)))
                .GroupBy(c => c.RecipeId)
                .ToDictionary(g => g.Key, g => g.Count());

            var views = (await _userRecipeViewRepository.GetAllAsync(v =>
                v.UserId == userId && v.ViewedAtUtc >= now.AddDays(-14)))
                .GroupBy(v => v.RecipeId)
                .ToDictionary(g => g.Key, g => g.Count());

            var savedIds = (await _userSaveRecipeRepository.GetAllAsync(s => s.UserId == userId))
                .Select(s => s.RecipeId)
                .ToHashSet();

            var userCtx = new RecommendationUserContext
            {
                UserId = userId,
                Tdee = tdee.Value,
                Targets = domainTargets,
                RestrictedIngredientIds = restrictions
                    .Where(r => r.IngredientId.HasValue)
                    .Select(r => r.IngredientId!.Value)
                    .ToHashSet(),
                RestrictedCategoryIds = restrictions
                    .Where(r => r.IngredientCategoryId.HasValue)
                    .Select(r => r.IngredientCategoryId!.Value)
                    .ToHashSet(),
                RatingByLabel = RecommendationBehaviorBuilder.BuildRatingByLabel(ratings, recipeLabelsMap),
                ViewByLabel = RecommendationBehaviorBuilder.BuildViewByLabel(views, recipeLabelsMap),
                CommentByLabel = RecommendationBehaviorBuilder.BuildCommentByLabel(comments, recipeLabelsMap),
                SaveByLabel = RecommendationBehaviorBuilder.BuildSaveByLabel(savedIds, recipeLabelsMap)
            };

            var mealTarget = _mealTargetProvider.BuildMealTarget(tdee.Value, mealType, domainTargets);

            var currentSnapshots = snapshots.Where(r => currentIds.Contains(r.Id)).ToList();
            var currentState = _mealNutritionCalculator.Calculate(currentSnapshots);
            var gap = _mealGapCalculator.Calculate(mealTarget, currentState);

            var candidates = snapshots.Where(r => !currentIds.Contains(r.Id));

            var suggested = _mealCompletionRecommender.Recommend(
                userCtx,
                mealTarget,
                currentState,
                gap,
                candidates,
                currentIds,
                Math.Max(1, request.SuggestionLimit))
                .ToList();

            var suggestedIds = suggested.Select(x => x.Recipe.Id).ToList();

            var fullRecipes = await _recipeRepository.GetAllAsync(
                r => suggestedIds.Contains(r.Id),
                q => q
                    .Include(r => r.Author).ThenInclude(a => a.Avatar)
                    .Include(r => r.Image)
                    .Include(r => r.Labels)
                    .Include(r => r.NutritionAggregates).ThenInclude(n => n.Nutrient)
            );

            var map = fullRecipes.ToDictionary(r => r.Id);
            var ordered = suggestedIds.Where(map.ContainsKey).Select(id => map[id]).ToList();
            var mapped = _mapper.Map<List<RecipeRankResponse>>(ordered);

            for (int i = 0; i < mapped.Count; i++)
                mapped[i].Score = suggested.First(x => x.Recipe.Id == ordered[i].Id).Score;

            var coverage = mealTarget.TargetCalories == 0
                ? 0
                : (double)(currentState.Calories / mealTarget.TargetCalories * 100);

            return new MealAnalyzeResponse
            {
                MealType = mealType,
                TargetCalories = mealTarget.TargetCalories,
                CurrentCalories = currentState.Calories,
                RemainingCalories = gap.RemainingCalories,
                EnergyCoveragePercent = coverage,
                TargetNutrients = mealTarget.NutrientTargets.ToDictionary(x => x.NutrientId, x => x.MinValue),
                CurrentNutrients = currentState.Nutrients.ToDictionary(x => x.Key, x => x.Value),
                RemainingNutrients = gap.RemainingNutrients.ToDictionary(
                    x => x.Key,
                    x => new NutrientRangeResponse
                    {
                        Min = x.Value.Min,
                        Max = x.Value.Max
                    }),
                Suggestions = mapped
            };
        }
    }
}
