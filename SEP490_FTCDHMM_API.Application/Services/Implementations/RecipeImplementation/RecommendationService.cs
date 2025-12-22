using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget;
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

        public RecommendationService(
            IUserRepository userRepository,
            IRecipeRepository recipeRepository,
            IMapper mapper,
            ICacheService cacheService,
            IRatingRepository ratingRepository,
            IUserRecipeViewRepository userRecipeViewRepository,
            ICommentRepository commentRepository,
            IUserSaveRecipeRepository userSaveRecipeRepository,
            IRecipeScoringSystem recipeScoringSystem)
        {
            _userRepository = userRepository;
            _recipeRepository = recipeRepository;
            _mapper = mapper;
            _ratingRepository = ratingRepository;
            _userRecipeViewRepository = userRecipeViewRepository;
            _commentRepository = commentRepository;
            _cacheService = cacheService;
            _userSaveRecipeRepository = userSaveRecipeRepository;
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

        public async Task<PagedResult<RecipeRankResponse>> ComputedCommendRecipesAsync(Guid userId)
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

            var fullRecipes = await _recipeRepository.Query()
                .AsNoTracking()
                .Where(r => recipeIds.Contains(r.Id))
                .Include(r => r.Author).ThenInclude(a => a.Avatar)
                .Include(r => r.Image)
                .Include(r => r.Labels)
                .Include(r => r.NutritionAggregates).ThenInclude(na => na.Nutrient)
                .ToListAsync();

            var recipeMap = fullRecipes.ToDictionary(r => r.Id);

            var result = cached
                .Where(x => recipeMap.ContainsKey(x.Id))
                .Select(x => {
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

                var fullCacheRecipes = await _recipeRepository.Query()
                    .AsNoTracking()
                    .Where(r => cacheRecipeIds.Contains(r.Id))
                    .Include(r => r.Author).ThenInclude(a => a.Avatar)
                    .Include(r => r.Image)
                    .Include(r => r.Labels)
                    .Include(r => r.NutritionAggregates).ThenInclude(na => na.Nutrient)
                    .ToListAsync();

                var recipeCacheMap = fullCacheRecipes.ToDictionary(r => r.Id);

                var cacheOrdered = cacheScored
                    .Where(x => recipeCacheMap.ContainsKey(x.Id))
                    .Select(x => new
                    {
                        Recipe = recipeCacheMap[x.Id],
                        x.Score,
                        x.UpdatedAtUtc
                    })
                    .ToList();

                var totalCacheCount = cacheOrdered.Count;

                var pagedCacheItems = cacheOrdered
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var cacheMapped = _mapper.Map<List<RecipeRankResponse>>(
                    pagedCacheItems.Select(x => x.Recipe).ToList());

                for (int i = 0; i < cacheMapped.Count; i++)
                    cacheMapped[i].Score = pagedCacheItems[i].Score;

                return new PagedResult<RecipeRankResponse>
                {
                    Items = cacheMapped,
                    TotalCount = totalCacheCount,
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

            var recipeLabelsMap = snapshots.ToDictionary(
                r => r.Id,
                r => r.LabelIds
            );

            var userBase = await _userRepository.Query()
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.Id,
                    Tdee = u.HealthMetrics
                        .OrderByDescending(h => h.RecordedAt)
                        .Select(h => (double?)h.TDEE)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();


            var systemTargets = await _userRepository.Query()
                .Where(u => u.Id == userId)
                .SelectMany(u => u.UserHealthGoals)
                .Where(hg =>
                    hg.Type == HealthGoalType.SYSTEM &&
                    hg.StartedAtUtc <= DateTime.UtcNow &&
                    (hg.ExpiredAtUtc == null || hg.ExpiredAtUtc > DateTime.UtcNow))
                .SelectMany(hg => hg.HealthGoal!.Targets)
                .Select(t => new NutrientTargetDto
                {
                    NutrientId = t.NutrientId,
                    MinValue = t.MinValue,
                    MaxValue = t.MaxValue,
                    MinEnergyPct = t.MinEnergyPct,
                    MaxEnergyPct = t.MaxEnergyPct,
                    TargetType = t.TargetType,
                    Weight = t.Weight
                })
                .ToListAsync();

            var customTargets = await _userRepository.Query()
                .Where(u => u.Id == userId)
                .SelectMany(u => u.UserHealthGoals)
                .Where(hg =>
                    hg.Type == HealthGoalType.CUSTOM &&
                    hg.StartedAtUtc <= DateTime.UtcNow &&
                    (hg.ExpiredAtUtc == null || hg.ExpiredAtUtc > DateTime.UtcNow))
                .SelectMany(hg => hg.CustomHealthGoal!.Targets)
                .Select(t => new NutrientTargetDto
                {
                    NutrientId = t.NutrientId,
                    MinValue = t.MinValue,
                    MaxValue = t.MaxValue,
                    MinEnergyPct = t.MinEnergyPct,
                    MaxEnergyPct = t.MaxEnergyPct,
                    TargetType = t.TargetType,
                    Weight = t.Weight
                })
                .ToListAsync();

            var restrictions = await _userRepository.Query()
                .Where(u => u.Id == userId)
                .SelectMany(u => u.DietRestrictions)
                .Where(r => r.ExpiredAtUtc == null || r.ExpiredAtUtc > DateTime.UtcNow)
                .ToListAsync();

            var ratings = await _ratingRepository.Query()
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .GroupBy(r => r.RecipeId)
                .Select(g => g
                    .OrderByDescending(x => x.CreatedAtUtc)
                    .Select(x => new { x.RecipeId, x.Score })
                    .First())
                .ToDictionaryAsync(x => x.RecipeId, x => x.Score);

            var commentCounts = await _commentRepository.Query()
                .AsNoTracking()
                .Where(c =>
                    c.UserId == userId &&
                    c.CreatedAtUtc >= DateTime.UtcNow.AddDays(-30))
                .GroupBy(c => c.RecipeId)
                .Select(g => new
                {
                    RecipeId = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.RecipeId, x => x.Count);

            var viewCounts = await _userRecipeViewRepository.Query()
                .AsNoTracking()
                .Where(v =>
                    v.UserId == userId &&
                    v.ViewedAtUtc >= DateTime.UtcNow.AddDays(-14))
                .GroupBy(v => v.RecipeId)
                .Select(g => new
                {
                    RecipeId = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.RecipeId, x => x.Count);


            var savedRecipeIds = await _userSaveRecipeRepository.Query()
                .Where(u => u.UserId == userId)
                .Select(s => s.RecipeId)
                .ToListAsync();

            var savedRecipeIdSet = savedRecipeIds.ToHashSet();

            var userCtx = new RecommendationUserContext
            {
                UserId = userBase!.Id,
                Tdee = userBase.Tdee ?? 0,
                Targets = systemTargets.Concat(customTargets).ToList(),
                RestrictedIngredientIds = restrictions
                    .Where(r => r.IngredientId.HasValue)
                    .Select(r => r.IngredientId!.Value)
                    .ToHashSet(),
                RestrictedCategoryIds = restrictions
                    .Where(r => r.IngredientCategoryId.HasValue)
                    .Select(r => r.IngredientCategoryId!.Value)
                    .ToHashSet(),
                RatingByLabel = RecommendationBehaviorBuilder.BuildRatingByLabel(ratings, recipeLabelsMap),
                ViewByLabel = RecommendationBehaviorBuilder.BuildViewByLabel(viewCounts, recipeLabelsMap),
                CommentByLabel = RecommendationBehaviorBuilder.BuildCommentByLabel(commentCounts, recipeLabelsMap),
                SaveByLabel = RecommendationBehaviorBuilder.BuildSaveByLabel(savedRecipeIdSet, recipeLabelsMap)
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

            await _cacheService.SetAsync(
                cacheKey,
                scored,
                TimeSpan.FromHours(1)
            );

            var recipeIds = scored.Select(x => x.Id).ToList();

            var fullRecipes = await _recipeRepository.Query()
                .AsNoTracking()
                .Where(r => recipeIds.Contains(r.Id))
                .Include(r => r.Author).ThenInclude(a => a.Avatar)
                .Include(r => r.Image)
                .Include(r => r.Labels)
                .Include(r => r.NutritionAggregates).ThenInclude(na => na.Nutrient)
                .ToListAsync();

            var recipeMap = fullRecipes.ToDictionary(r => r.Id);

            var ordered = scored
                .Where(x => recipeMap.ContainsKey(x.Id))
                .Select(x => new
                {
                    Recipe = recipeMap[x.Id],
                    x.Score,
                    x.UpdatedAtUtc
                })
                .ToList();

            var totalCount = ordered.Count;

            var pageItems = ordered
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var mapped = _mapper.Map<List<RecipeRankResponse>>(
                pageItems.Select(x => x.Recipe).ToList());

            for (int i = 0; i < mapped.Count; i++)
                mapped[i].Score = pageItems[i].Score;

            var result = new PagedResult<RecipeRankResponse>
            {
                Items = mapped,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            return result;
        }

    }
}
