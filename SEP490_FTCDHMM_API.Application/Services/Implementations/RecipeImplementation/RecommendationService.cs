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

        public RecommendationService(
            IUserRepository userRepository,
            IRecipeRepository recipeRepository,
            IMapper mapper,
            ICacheService cacheService,
            IRatingRepository ratingRepository,
            IUserRecipeViewRepository userRecipeViewRepository,
            ICommentRepository commentRepository,
            IRecipeScoringSystem recipeScoringSystem)
        {
            _userRepository = userRepository;
            _recipeRepository = recipeRepository;
            _mapper = mapper;
            _ratingRepository = ratingRepository;
            _userRecipeViewRepository = userRecipeViewRepository;
            _commentRepository = commentRepository;
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

        public async Task<PagedResult<RecipeRankResponse>> ComputedCommendRecipesAsync(Guid userId)
        {
            var meal = GetCurrentMeal().ToString().ToLower();
            var cacheKey = $"recommend:user:{userId}:meal:{meal}";

            var cachedPage = await _cacheService.GetAsync<PagedResult<RecipeRankResponse>>(cacheKey);
            if (cachedPage != null)
            {
                foreach (var item in cachedPage.Items)
                    item.Score = null;

                return cachedPage;
            }

            return new PagedResult<RecipeRankResponse>
            {
                Items = new List<RecipeRankResponse>(),
                TotalCount = 0,
                PageNumber = 0,
                PageSize = 0
            };
        }

        public async Task<PagedResult<RecipeRankResponse>> RecommendRecipesAsync(Guid userId, PaginationParams request)
        {
            var meal = GetCurrentMeal().ToString().ToLower();
            var cacheKey = $"recommend:user:{userId}:meal:{meal}:page:{request.PageNumber}";

            var cachedPage = await _cacheService.GetAsync<PagedResult<RecipeRankResponse>>(cacheKey);
            if (cachedPage != null)
            {
                foreach (var item in cachedPage.Items)
                    item.Score = null;

                return cachedPage;
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


            var savedRecipeIds = await _userRepository.Query()
                .Where(u => u.Id == userId)
                .SelectMany(u => u.SaveRecipes)
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
                .Select(r => new
                {
                    r.Id,
                    r.UpdatedAtUtc,
                    Score = _recipeScoringSystem.CalculateFinalScore(userCtx, r)
                })
                .OrderByDescending(x => x.Score)
                .Take(request.PageSize * 5)
                .ToList();

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
                .OrderByDescending(x => x.Score)
                    .ThenByDescending(x => x.UpdatedAtUtc)
                .ToList();

            var totalCount = snapshots.Count;

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

            await _cacheService.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromHours(1)
            );

            return result;
        }

    }
}

