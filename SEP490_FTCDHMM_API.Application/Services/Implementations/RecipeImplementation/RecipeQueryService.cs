using System.Text.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Rating;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.UserSaveRecipe;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Specifications;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation
{
    public class RecipeQueryService : IRecipeQueryService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserSaveRecipeRepository _userSaveRecipeRepository;
        private readonly IMapper _mapper;
        private readonly IUserRecipeViewRepository _userRecipeViewRepository;
        private readonly ICacheService _cacheService;
        public RecipeQueryService(
            IRecipeRepository recipeRepository,
            IUserRepository userRepository,
            IUserSaveRecipeRepository userSaveRecipeRepository,
            IUserRecipeViewRepository userRecipeViewRepository,
            ICacheService cacheService,
            IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _userRepository = userRepository;
            _userSaveRecipeRepository = userSaveRecipeRepository;
            _mapper = mapper;
            _userRecipeViewRepository = userRecipeViewRepository;
            _cacheService = cacheService;
        }

        public async Task<PagedResult<RecipeResponse>> GetRecipesAsync(RecipeFilterRequest request)
        {
            var cacheKey = $"recipe:list:{JsonSerializer.Serialize(request)}";
            var cached = await _cacheService.GetAsync<PagedResult<RecipeResponse>>(cacheKey);
            if (cached != null)
                return cached;

            var spec = new RecipeBasicFilterSpec
            {
                IncludeIngredientIds = request.IncludeIngredientIds,
                ExcludeIngredientIds = request.ExcludeIngredientIds,
                IncludeLabelIds = request.IncludeLabelIds,
                ExcludeLabelIds = request.ExcludeLabelIds,
                Difficulty = request.Difficulty,
                Keyword = request.Keyword.NormalizeVi(),
                Ration = request.Ration,
                MaxCookTime = request.MaxCookTime
            };

            var sources = await _recipeRepository.GetRecipesForRankingAsync(spec);

            var ranked = sources
                .Select(r => new
                {
                    r.RecipeId,
                    Matched = request.IncludeIngredientIds.Any()
                        ? r.IngredientIds.Count(id => request.IncludeIngredientIds.Contains(id))
                        : 0,
                    NotMatched = request.IncludeIngredientIds.Any()
                        ? request.IncludeIngredientIds.Count(id => !r.IngredientIds.Contains(id))
                        : 0,
                    r.UpdatedAtUtc
                })
                .Where(x => !request.IncludeIngredientIds.Any() || x.Matched > 0)
                .OrderByDescending(x => x.Matched)
                .ThenBy(x => x.NotMatched)
                .ThenByDescending(x => x.UpdatedAtUtc);

            var totalCount = ranked.Count();

            var pageIds = ranked
                .Skip((request.PaginationParams.PageNumber - 1) * request.PaginationParams.PageSize)
                .Take(request.PaginationParams.PageSize)
                .Select(x => x.RecipeId)
                .ToList();

            var recipes = await _recipeRepository.GetAllAsync(
                r => pageIds.Contains(r.Id),
                include: q => q
                    .Include(r => r.Image)
                    .Include(r => r.Author).ThenInclude(u => u.Avatar)
            );

            var orderedRecipes = pageIds
                .Join(recipes, id => id, r => r.Id, (_, r) => r)
                .ToList();

            var result = _mapper.Map<List<RecipeResponse>>(orderedRecipes);

            var pagedResult = new PagedResult<RecipeResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = request.PaginationParams.PageNumber,
                PageSize = request.PaginationParams.PageSize
            };

            await _cacheService.SetAsync(cacheKey, pagedResult, TimeSpan.FromMinutes(5));

            return pagedResult;
        }


        public async Task<RecipeDetailsResponse> GetRecipeDetailsAsync(Guid? userId, Guid recipeId)
        {
            if (!userId.HasValue)
            {
                var cacheKey = $"recipe:detail:{recipeId}";
                var cached = await _cacheService.GetAsync<RecipeDetailsResponse>(cacheKey);
                if (cached != null)
                    return cached;
            }

            IQueryable<Recipe> IncludeRecipeDetails(IQueryable<Recipe> q) =>
                q.Include(r => r.Author).ThenInclude(u => u.Avatar)
                 .Include(r => r.Image)
                 .Include(r => r.Labels)
                 .Include(r => r.Parent)
                 .Include(r => r.RecipeUserTags).ThenInclude(t => t.TaggedUser)
                 .Include(r => r.CookingSteps)
                     .ThenInclude(cs => cs.CookingStepImages)
                         .ThenInclude(i => i.Image)
                 .Include(r => r.RecipeIngredients)
                     .ThenInclude(ri => ri.Ingredient)
                         .ThenInclude(i => i.Categories);

            var recipe = await _recipeRepository.GetByIdAsync(recipeId, IncludeRecipeDetails);

            await ValidateAccessRecipeAsync(userId, recipe);

            var now = DateTime.UtcNow;

            if (!userId.HasValue)
            {
                return _mapper.Map<RecipeDetailsResponse>(recipe);
            }

            var uid = userId.Value;
            var isOwner = recipe!.AuthorId == uid;

            if (!isOwner)
            {
                var existingView = await _userRecipeViewRepository.ExistsAsync(v =>
                    v.UserId == uid && v.RecipeId == recipeId);

                if (!existingView)
                {
                    await _userRecipeViewRepository.AddAsync(new RecipeUserView
                    {
                        UserId = uid,
                        RecipeId = recipeId,
                        ViewedAtUtc = now
                    });

                    recipe.ViewCount = await _userRecipeViewRepository.CountAsync(v => v.RecipeId == recipeId);
                    await _recipeRepository.UpdateAsync(recipe);
                }
            }

            var user = await _userRepository.GetByIdAsync(
                uid,
                include: q => q.Include(u => u.DietRestrictions)
            );

            var isSaved = await _userSaveRecipeRepository.ExistsAsync(s =>
                s.UserId == uid && s.RecipeId == recipeId);

            var activeRestrictions = user!.DietRestrictions
                .Where(r => !r.ExpiredAtUtc.HasValue || r.ExpiredAtUtc > now)
                .ToList();

            var result = _mapper.Map<RecipeDetailsResponse>(recipe);

            if (!userId.HasValue)
            {
                await _cacheService.SetAsync(
                    $"recipe:detail:{recipeId}",
                    result,
                    TimeSpan.FromMinutes(10)
                );
                return result;
            }

            result.IsSaved = isSaved;

            if (!activeRestrictions.Any())
                return result;

            var ingredientLookup = recipe.RecipeIngredients
                .ToDictionary(x => x.IngredientId, x => x.Ingredient);

            var ingredientRestrictions = activeRestrictions
                .Where(r => r.IngredientId.HasValue)
                .GroupBy(r => r.IngredientId!.Value)
                .ToDictionary(g => g.Key, g => g.First().Type);

            var categoryRestrictions = activeRestrictions
                .Where(r => r.IngredientCategoryId.HasValue)
                .GroupBy(r => r.IngredientCategoryId!.Value)
                .ToDictionary(g => g.Key, g => g.First().Type);

            foreach (var ing in result.Ingredients)
            {
                RestrictionType? selected = null;

                if (ingredientRestrictions.TryGetValue(ing.IngredientId, out var directType))
                {
                    selected = directType;
                }

                if (ingredientLookup.TryGetValue(ing.IngredientId, out var ingredientEntity))
                {
                    foreach (var category in ingredientEntity.Categories)
                    {
                        if (categoryRestrictions.TryGetValue(category.Id, out var catType))
                        {
                            if (selected == null ||
                                GetRestrictionPriority(catType) > GetRestrictionPriority(selected))
                            {
                                selected = catType;
                            }
                        }
                    }
                }

                if (selected != null)
                {
                    ing.RestrictionType = selected;
                }
            }


            return result;
        }

        private static int GetRestrictionPriority(RestrictionType type)
        {
            if (type == RestrictionType.Allergy) return 3;
            if (type == RestrictionType.Dislike) return 2;
            if (type == RestrictionType.TemporaryAvoid) return 1;
            return 0;
        }

        public async Task<PagedResult<RecipeResponse>> GetSavedRecipesAsync(Guid userId, SaveRecipeFilterRequest request)
        {
            var (items, totalCount) = await _userSaveRecipeRepository.GetPagedAsync(
                pageNumber: request.PaginationParams.PageNumber,
                pageSize: request.PaginationParams.PageSize,
                filter: f => f.UserId == userId && f.Recipe.Status == RecipeStatus.Posted,
                orderBy: q => q.OrderByDescending(f => f.CreatedAtUtc),
                include: q => q
                    .Include(f => f.Recipe).ThenInclude(r => r.Author).ThenInclude(u => u.Avatar)
                    .Include(f => f.Recipe.Image)
                );

            var normalizedKeyword = request.Keyword?.NormalizeVi() ?? string.Empty;

            var recipes = items.Select(f => f.Recipe).Where(r => r.NormalizedName.Contains(normalizedKeyword)).ToList();
            var result = _mapper.Map<List<RecipeResponse>>(recipes);

            return new PagedResult<RecipeResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = request.PaginationParams.PageNumber,
                PageSize = request.PaginationParams.PageSize
            };
        }

        public async Task<PagedResult<MyRecipeResponse>> GetRecipesByUserIdAsync(Guid userId, RecipePaginationParams paginationParams)
        {
            Func<IQueryable<Recipe>, IQueryable<Recipe>> include = q =>
                 q.Include(r => r.Image)
                  .Include(r => r.Labels)
                  .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                  .Include(r => r.RecipeUserTags).ThenInclude(cs => cs.TaggedUser)
                  .Include(r => r.CookingSteps).ThenInclude(cs => cs.CookingStepImages).ThenInclude(cs => cs.Image);

            var (items, totalCount) = await _recipeRepository.GetPagedAsync(
                pageNumber: paginationParams.PageNumber,
                pageSize: paginationParams.PageSize,
                filter: f => f.Status == RecipeStatus.Posted && f.AuthorId == userId,
                orderBy: o => o.OrderByDescending(r => r.CreatedAtUtc),
                include: include
            );

            var result = _mapper.Map<List<MyRecipeResponse>>(items);

            return new PagedResult<MyRecipeResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize
            };
        }

        public async Task<PagedResult<MyRecipeResponse>> GetRecipesByUserNameAsync(string userName, RecipePaginationParams paginationParams)
        {
            var user = await _userRepository.GetByUserNameAsync(userName);
            if (user == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Người dùng không tồn tại");

            Func<IQueryable<Recipe>, IQueryable<Recipe>> include = q =>
                 q.Include(r => r.Image)
                  .Include(r => r.Labels)
                  .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                  .Include(r => r.RecipeUserTags).ThenInclude(cs => cs.TaggedUser)
                  .Include(r => r.CookingSteps).ThenInclude(cs => cs.CookingStepImages).ThenInclude(cs => cs.Image);

            var (items, totalCount) = await _recipeRepository.GetPagedAsync(
                pageNumber: paginationParams.PageNumber,
                pageSize: paginationParams.PageSize,
                filter: f => f.Status == RecipeStatus.Posted && f.AuthorId == user.Id,
                orderBy: o => o.OrderByDescending(r => r.CreatedAtUtc),
                include: include
            );

            var result = _mapper.Map<List<MyRecipeResponse>>(items);

            return new PagedResult<MyRecipeResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize
            };
        }

        public async Task<RecipeRatingResponse> GetRecipeRatingsAsync(Guid? userId, Guid recipeId)
        {
            if (!userId.HasValue)
            {
                var cacheKey = $"recipe:rating:{recipeId}";
                var cached = await _cacheService.GetAsync<RecipeRatingResponse>(cacheKey);
                if (cached != null)
                    return cached;
            }

            var recipe = await _recipeRepository.GetByIdAsync(recipeId);

            await ValidateAccessRecipeAsync(userId, recipe);

            var result = _mapper.Map<RecipeRatingResponse>(recipe);

            if (!userId.HasValue)
            {
                await _cacheService.SetAsync(
                    $"recipe:rating:{recipeId}",
                    result,
                    TimeSpan.FromMinutes(10)
                );
                return result;
            }
            return result;
        }

        public async Task<PagedResult<RatingDetailsResponse>> GetRecipeRatingDetailsAsync(Guid? userId, Guid recipeId, RecipePaginationParams request)
        {
            var recipe = await _recipeRepository.GetByIdAsync(
                id: recipeId,
                include: i => i.Include(r => r.Ratings)
                               .ThenInclude(rt => rt.User)
                               .ThenInclude(u => u.Avatar));

            await ValidateAccessRecipeAsync(userId, recipe);

            var query = recipe!.Ratings
                .OrderByDescending(v => v.CreatedAtUtc)
                .AsQueryable();

            var totalCount = query.Count();

            var pagedRatings = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var result = _mapper.Map<IEnumerable<RatingDetailsResponse>>(pagedRatings);

            foreach (var rating in result)
            {
                if (rating.UserInteractionResponse.Id == userId)
                {
                    rating.IsOwner = true;
                }
            }

            return new PagedResult<RatingDetailsResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PagedResult<RecipeResponse>> GetRecipeHistoriesAsync(Guid userId, RecipePaginationParams request)
        {
            var (items, totalCount) = await _userRecipeViewRepository.GetPagedAsync(
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                filter: v => v.UserId == userId && v.Recipe.Status == RecipeStatus.Posted,
                orderBy: o => o.OrderByDescending(v => v.ViewedAtUtc),
                include: i => i.Include(v => v.Recipe)
                               .ThenInclude(r => r.Image)
                               .Include(v => v.Recipe.Author).ThenInclude(a => a.Avatar)
            );

            var recipes = items.Select(v => v.Recipe).ToList();

            var result = _mapper.Map<IEnumerable<RecipeResponse>>(recipes);

            return new PagedResult<RecipeResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PagedResult<RecipeManagementResponse>> GetRecipePendingsAsync(PaginationParams request)
        {
            Func<IQueryable<Recipe>, IQueryable<Recipe>> include = q =>
                q.Include(r => r.Author).ThenInclude(u => u.Avatar)
                 .Include(r => r.Image)
                 .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                 .Include(r => r.Labels)
                 .Include(r => r.RecipeUserTags).ThenInclude(cs => cs.TaggedUser)
                 .Include(r => r.CookingSteps).ThenInclude(cs => cs.CookingStepImages).ThenInclude(cs => cs.Image);

            var (items, totalCount) = await _recipeRepository.GetPagedAsync(
                filter: f => f.Status == RecipeStatus.Pending,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                orderBy: o => o.OrderByDescending(r => r.UpdatedAtUtc),
                include: include
            );

            var result = _mapper.Map<IReadOnlyList<RecipeManagementResponse>>(items);

            return new PagedResult<RecipeManagementResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PagedResult<RecipeManagementResponse>> GetRecipePendingsByUserIdAsync(Guid userId, PaginationParams request)
        {
            Func<IQueryable<Recipe>, IQueryable<Recipe>> include = q =>
                q.Include(r => r.Author).ThenInclude(u => u.Avatar)
                 .Include(r => r.Image)
                 .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                 .Include(r => r.Labels)
                 .Include(r => r.RecipeUserTags).ThenInclude(cs => cs.TaggedUser)
                 .Include(r => r.CookingSteps).ThenInclude(cs => cs.CookingStepImages).ThenInclude(cs => cs.Image);

            var (items, totalCount) = await _recipeRepository.GetPagedAsync(
                filter: f => (f.Status == RecipeStatus.Pending || f.Status == RecipeStatus.Locked) && f.AuthorId == userId,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                orderBy: o => o.OrderByDescending(r => r.UpdatedAtUtc),
                include: include
            );

            var result = _mapper.Map<IReadOnlyList<RecipeManagementResponse>>(items);

            return new PagedResult<RecipeManagementResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        private async Task ValidateAccessRecipeAsync(Guid? userId, Recipe? recipe)
        {
            if (recipe == null || recipe.Status == RecipeStatus.Deleted)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (recipe.Status != RecipeStatus.Posted)
            {
                if (!userId.HasValue)
                    throw new AppException(AppResponseCode.NOT_FOUND);

                var user = await _userRepository.GetByIdAsync(userId.Value,
                    include: i => i.Include(u => u.Role)
                                        .ThenInclude(r => r.RolePermissions)
                                            .ThenInclude(rp => rp.PermissionAction)
                                                .ThenInclude(pa => pa.PermissionDomain)
                    );

                var isAuthor = recipe.AuthorId == userId.Value;

                var havePermission = user!.Role.RolePermissions
                    .Select(rp => rp.PermissionAction)
                    .Any(pa =>
                        pa.PermissionDomain.Name == PermissionValue.Recipe_ManagementView.Domain &&
                        pa.Name == PermissionValue.Recipe_ManagementView.Action);

                if (!(isAuthor || havePermission))
                    throw new AppException(AppResponseCode.NOT_FOUND);
            }
        }
    }
}
