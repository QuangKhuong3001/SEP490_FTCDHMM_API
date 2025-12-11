using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Rating;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.UserSaveRecipe;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
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
        private readonly IIngredientRepository _ingredientRepository;
        private readonly ILabelRepository _labelRepository;
        private readonly IUserRecipeViewRepository _userRecipeViewRepository;

        public RecipeQueryService(
            IRecipeRepository recipeRepository,
            IUserRepository userRepository,
            IUserSaveRecipeRepository userSaveRecipeRepository,
            IIngredientRepository ingredientRepository,
            ILabelRepository labelRepository,
            IUserRecipeViewRepository userRecipeViewRepository,
            IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _userRepository = userRepository;
            _userSaveRecipeRepository = userSaveRecipeRepository;
            _mapper = mapper;
            _userRecipeViewRepository = userRecipeViewRepository;
            _ingredientRepository = ingredientRepository;
            _labelRepository = labelRepository;
        }

        public async Task<PagedResult<RecipeResponse>> GetRecipesAsync(RecipeFilterRequest request)
        {
            if (request.IncludeIngredientIds.Any())
            {
                var inIngredientExist = await _ingredientRepository.IdsExistAsync(request.IncludeIngredientIds);
                if (!inIngredientExist)
                    throw new AppException(AppResponseCode.NOT_FOUND, "Một hoặc nhiều nguyên liệu trong danh sách bao gồm không tồn tại trong hệ thống");
            }

            if (request.ExcludeIngredientIds.Any())
            {
                var exIngredientExist = await _ingredientRepository.IdsExistAsync(request.ExcludeIngredientIds);
                if (!exIngredientExist)
                    throw new AppException(AppResponseCode.NOT_FOUND, "Một hoặc nhiều nguyên liệu trong danh sách loại trừ không tồn tại trong hệ thống");
            }

            if (request.IncludeLabelIds.Any())
            {
                var inLabelExist = await _labelRepository.IdsExistAsync(request.IncludeLabelIds);
                if (!inLabelExist)
                    throw new AppException(AppResponseCode.NOT_FOUND, "Một hoặc nhiều nhãn trong danh sách bao gồm không tồn tại trong hệ thống");
            }

            if (request.ExcludeLabelIds.Any())
            {
                var exLabelExist = await _labelRepository.IdsExistAsync(request.ExcludeLabelIds);
                if (!exLabelExist)
                    throw new AppException(AppResponseCode.NOT_FOUND, "Một hoặc nhiều nhãn trong danh sách loại trừ không tồn tại trong hệ thống");
            }

            if (request.IncludeIngredientIds.Any() && request.ExcludeIngredientIds.Any())
            {
                var conflictIngredientIds = request.IncludeIngredientIds
                    .Intersect(request.ExcludeIngredientIds)
                    .ToList();

                if (conflictIngredientIds.Any())
                    throw new AppException(
                        AppResponseCode.INVALID_ACTION,
                        "Không thể đồng thời bao gồm và loại trừ cùng một nguyên liệu"
                    );
            }

            if (request.IncludeLabelIds.Any() && request.ExcludeLabelIds.Any())
            {
                var conflictLabelIds = request.IncludeLabelIds
                    .Intersect(request.ExcludeLabelIds)
                    .ToList();

                if (conflictLabelIds.Any())
                    throw new AppException(
                        AppResponseCode.INVALID_ACTION,
                        "Không thể đồng thời bao gồm và loại trừ cùng một nhãn"
                    );
            }

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

            var recipes = await _recipeRepository.GetRecipesRawAsync(spec);

            var hasIncludeIngredient = request.IncludeIngredientIds.Any();

            var ranked = recipes
                .Select(r => new
                {
                    Recipe = r,
                    Matched = request.IncludeIngredientIds.Any()
                        ? r.RecipeIngredients.Count(ri =>
                              request.IncludeIngredientIds.Contains(ri.IngredientId))
                        : 0,

                    NotMatched = request.IncludeIngredientIds.Any()
                        ? request.IncludeIngredientIds.Count(id =>
                              !r.RecipeIngredients.Any(ri => ri.IngredientId == id))
                        : 0
                })
                .Where(x => !hasIncludeIngredient || x.Matched > 0)
                .OrderByDescending(x => x.Matched)
                .ThenBy(x => x.NotMatched);

            var ordered = request.SortBy?.ToLower() switch
            {
                "name_asc" => ranked.ThenBy(x => x.Recipe.Name),
                "name_desc" => ranked.ThenByDescending(x => x.Recipe.Name),
                "time_asc" => ranked.ThenBy(x => x.Recipe.CookTime),
                "time_desc" => ranked.ThenByDescending(x => x.Recipe.CookTime),
                "latest" => ranked.ThenByDescending(x => x.Recipe.UpdatedAtUtc),
                "rate_asc" => ranked.ThenBy(x => x.Recipe.AvgRating),
                "rate_desc" => ranked.ThenByDescending(x => x.Recipe.AvgRating),
                "view_asc" => ranked.ThenBy(x => x.Recipe.ViewCount),
                "view_desc" => ranked.ThenByDescending(x => x.Recipe.ViewCount),
                _ => ranked
            };

            var paged = ordered
                .Skip((request.PaginationParams.PageNumber - 1) * request.PaginationParams.PageSize)
                .Take(request.PaginationParams.PageSize)
                .Select(x => x.Recipe)
                .ToList();

            var result = _mapper.Map<List<RecipeResponse>>(paged);

            return new PagedResult<RecipeResponse>
            {
                Items = result,
                TotalCount = ordered.Count(),
                PageNumber = request.PaginationParams.PageNumber,
                PageSize = request.PaginationParams.PageSize
            };
        }

        public async Task<RecipeDetailsResponse> GetRecipeDetailsAsync(Guid? userId, Guid recipeId)
        {
            IQueryable<Recipe> IncludeRecipeDetails(IQueryable<Recipe> q) =>
                q.Include(r => r.Author).ThenInclude(u => u.Avatar)
                 .Include(r => r.Image)
                 .Include(r => r.Labels)
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
                await _userRecipeViewRepository.AddAsync(new UserRecipeView
                {
                    UserId = uid,
                    RecipeId = recipeId,
                    ViewedAtUtc = now
                });

                recipe.ViewCount = await _userRecipeViewRepository.CountAsync(v => v.RecipeId == recipeId);
                await _recipeRepository.UpdateAsync(recipe);
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
                if (ingredientRestrictions.TryGetValue(ing.IngredientId, out var directType))
                {
                    ing.RestrictionType = directType;
                    continue;
                }

                if (!ingredientLookup.TryGetValue(ing.IngredientId, out var ingredientEntity))
                    continue;

                foreach (var category in ingredientEntity.Categories)
                {
                    if (categoryRestrictions.TryGetValue(category.Id, out var catType))
                    {
                        ing.RestrictionType = catType;
                        break;
                    }
                }
            }

            return result;
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
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);

            await ValidateAccessRecipeAsync(userId, recipe);

            var result = _mapper.Map<RecipeRatingResponse>(recipe);
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

            var result = _mapper.Map<IEnumerable<RecipeResponse>>(items);

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
