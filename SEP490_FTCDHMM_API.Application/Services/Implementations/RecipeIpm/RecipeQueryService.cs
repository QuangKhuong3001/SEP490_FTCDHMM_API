using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Rating;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.UserFavoriteRecipe;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.UserSaveRecipe;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeIpm
{
    public class RecipeQueryService : IRecipeQueryService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserFavoriteRecipeRepository _userFavoriteRecipeRepository;
        private readonly IUserSaveRecipeRepository _userSaveRecipeRepository;
        private readonly IUserRecipeViewRepository _userRecipeViewRepository;
        private readonly IMapper _mapper;
        private readonly IRecipeBehaviorService _behaviorService;

        public RecipeQueryService(
            IRecipeRepository recipeRepository,
            IUserRepository userRepository,
            IUserFavoriteRecipeRepository userFavoriteRecipeRepository,
            IUserSaveRecipeRepository userSaveRecipeRepository,
            IUserRecipeViewRepository userRecipeViewRepository,
            IMapper mapper,
            IRecipeBehaviorService behaviorService)
        {
            _recipeRepository = recipeRepository;
            _userRepository = userRepository;
            _userFavoriteRecipeRepository = userFavoriteRecipeRepository;
            _userSaveRecipeRepository = userSaveRecipeRepository;
            _userRecipeViewRepository = userRecipeViewRepository;
            _mapper = mapper;
            _behaviorService = behaviorService;
        }

        public async Task<PagedResult<RecipeResponse>> GetAllRecipesAsync(RecipeFilterRequest request)
        {
            Expression<Func<Recipe, bool>> filter = f =>
                !f.IsDeleted
                && (request.Difficulty == null || f.Difficulty == DifficultyValue.From(request.Difficulty))
                && (request.Ration == null || f.Ration == request.Ration)
                && (request.MaxCookTime == null || f.CookTime < request.MaxCookTime)
                && (!request.LabelIds.Any() || f.Labels.Any(l => request.LabelIds.Contains(l.Id)))
                && (!request.IngredientIds.Any() || f.RecipeIngredients.Any(ri => request.IngredientIds.Contains(ri.IngredientId)))
                && (string.IsNullOrEmpty(request.Keyword) || f.Name.Contains(request.Keyword));

            Func<IQueryable<Recipe>, IOrderedQueryable<Recipe>> orderBy = request.SortBy?.ToLower() switch
            {
                "name_asc" => q => q.OrderBy(r => r.Name),
                "name_desc" => q => q.OrderByDescending(r => r.Name),
                "time_asc" => q => q.OrderBy(r => r.CookTime),
                "time_desc" => q => q.OrderByDescending(r => r.CookTime),
                "latest" => q => q.OrderByDescending(r => r.UpdatedAtUtc),
                "rate_asc" => q => q.OrderBy(r => r.AvgRating),
                "rate_desc" => q => q.OrderByDescending(r => r.AvgRating),
                "view_asc" => q => q.OrderBy(r => r.ViewCount),
                "view_desc" => q => q.OrderByDescending(r => r.ViewCount),
                _ => q => q.OrderByDescending(r => r.UpdatedAtUtc)
            };

            Func<IQueryable<Recipe>, IQueryable<Recipe>> include = q =>
                q.Include(r => r.Author).ThenInclude(u => u.Avatar)
                 .Include(r => r.Image)
                 .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                 .Include(r => r.Labels)
                 .Include(r => r.RecipeUserTags).ThenInclude(cs => cs.TaggedUser)
                 .Include(r => r.CookingSteps).ThenInclude(cs => cs.CookingStepImages).ThenInclude(cs => cs.Image);

            var (items, totalCount) = await _recipeRepository.GetPagedAsync(
                pageNumber: request.PaginationParams.PageNumber,
                pageSize: request.PaginationParams.PageSize,
                filter: filter,
                orderBy: orderBy,
                keyword: request.Keyword,
                searchProperties: new[] { "Name", "Description" },
                include: include
            );

            var result = _mapper.Map<IReadOnlyList<RecipeResponse>>(items);

            return new PagedResult<RecipeResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = request.PaginationParams.PageNumber,
                PageSize = request.PaginationParams.PageSize
            };
        }

        public async Task<RecipeDetailsResponse> GetRecipeDetailsAsync(Guid userId, Guid recipeId)
        {
            IQueryable<Recipe> include(IQueryable<Recipe> q) =>
                q.Include(r => r.Author).ThenInclude(u => u.Avatar)
                 .Include(r => r.Image)
                 .Include(r => r.Labels)
                 .Include(r => r.Parent)
                 .Include(r => r.RecipeUserTags).ThenInclude(cs => cs.TaggedUser)
                 .Include(r => r.CookingSteps).ThenInclude(cs => cs.CookingStepImages).ThenInclude(cs => cs.Image)
                 .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient);

            var recipe = await _recipeRepository.GetByIdAsync(recipeId, include);

            if (recipe == null || recipe.IsDeleted)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            await _behaviorService.RecordViewAsync(userId, recipe);
            await _recipeRepository.UpdateAsync(recipe);

            var isFavorited = await _userFavoriteRecipeRepository.ExistsAsync(f => f.UserId == userId && f.RecipeId == recipeId);
            var isSaved = await _userSaveRecipeRepository.ExistsAsync(s => s.UserId == userId && s.RecipeId == recipeId);

            var result = _mapper.Map<RecipeDetailsResponse>(recipe);
            result.IsFavorited = isFavorited;
            result.IsSaved = isSaved;

            return result;
        }

        public async Task<PagedResult<RecipeResponse>> GetFavoriteListAsync(Guid userId, FavoriteRecipeFilterRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var (items, totalCount) = await _userFavoriteRecipeRepository.GetPagedAsync(
                pageNumber: request.PaginationParams.PageNumber,
                pageSize: request.PaginationParams.PageSize,
                filter: f => f.UserId == userId && f.Recipe.IsDeleted == false,
                orderBy: q => q.OrderByDescending(f => f.CreatedAtUtc),
                include: q => q
                    .Include(f => f.Recipe).ThenInclude(r => r.Author).ThenInclude(u => u.Avatar)
                    .Include(f => f.Recipe.Image)
                    .Include(f => f.Recipe.FavoritedBy)
            );

            var recipes = items.Select(f => f.Recipe).ToList();
            var result = _mapper.Map<List<RecipeResponse>>(recipes);

            return new PagedResult<RecipeResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = request.PaginationParams.PageNumber,
                PageSize = request.PaginationParams.PageSize
            };
        }

        public async Task<PagedResult<RecipeResponse>> GetSavedListAsync(Guid userId, SaveRecipeFilterRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var (items, totalCount) = await _userSaveRecipeRepository.GetPagedAsync(
                pageNumber: request.PaginationParams.PageNumber,
                pageSize: request.PaginationParams.PageSize,
                filter: f => f.UserId == userId && f.Recipe.IsDeleted == false,
                orderBy: q => q.OrderByDescending(f => f.CreatedAtUtc),
                include: q => q
                    .Include(f => f.Recipe).ThenInclude(r => r.Author).ThenInclude(u => u.Avatar)
                    .Include(f => f.Recipe.Image)
                    .Include(f => f.Recipe.FavoritedBy)
            );

            var recipes = items.Select(f => f.Recipe).ToList();
            var result = _mapper.Map<List<RecipeResponse>>(recipes);

            return new PagedResult<RecipeResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = request.PaginationParams.PageNumber,
                PageSize = request.PaginationParams.PageSize
            };
        }

        public async Task<PagedResult<MyRecipeResponse>> GetRecipeByUserIdAsync(Guid userId, RecipePaginationParams paginationParams)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            Func<IQueryable<Recipe>, IQueryable<Recipe>> include = q =>
                 q.Include(r => r.Image)
                  .Include(r => r.Labels)
                  .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                  .Include(r => r.RecipeUserTags).ThenInclude(cs => cs.TaggedUser)
                  .Include(r => r.CookingSteps).ThenInclude(cs => cs.CookingStepImages).ThenInclude(cs => cs.Image);

            var (items, totalCount) = await _recipeRepository.GetPagedAsync(
                pageNumber: paginationParams.PageNumber,
                pageSize: paginationParams.PageSize,
                filter: f => !f.IsDeleted && f.AuthorId == userId,
                orderBy: o => o.OrderByDescending(r => r.CreatedAtUtc),
                include: include
            );

            var result = _mapper.Map<IReadOnlyList<MyRecipeResponse>>(items);

            return new PagedResult<MyRecipeResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize
            };
        }

        public async Task<PagedResult<MyRecipeResponse>> GetRecipeByUserNameAsync(string userName, RecipePaginationParams paginationParams)
        {
            var user = await _userRepository.GetByUserNameAsync(userName);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION, "Người dùng không tồn tại");

            Func<IQueryable<Recipe>, IQueryable<Recipe>> include = q =>
                 q.Include(r => r.Image)
                  .Include(r => r.Labels)
                  .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                  .Include(r => r.RecipeUserTags).ThenInclude(cs => cs.TaggedUser)
                  .Include(r => r.CookingSteps).ThenInclude(cs => cs.CookingStepImages).ThenInclude(cs => cs.Image);

            var (items, totalCount) = await _recipeRepository.GetPagedAsync(
                pageNumber: paginationParams.PageNumber,
                pageSize: paginationParams.PageSize,
                filter: f => !f.IsDeleted && f.AuthorId == user.Id,
                orderBy: o => o.OrderByDescending(r => r.CreatedAtUtc),
                include: include
            );

            var result = _mapper.Map<IReadOnlyList<MyRecipeResponse>>(items);

            return new PagedResult<MyRecipeResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize
            };
        }

        public async Task<RecipeRatingResponse> GetRecipeRatingAsync(Guid recipeId)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null || recipe.IsDeleted)
                throw new AppException(AppResponseCode.NOT_FOUND, "Công thức không tồn tại");

            var result = _mapper.Map<RecipeRatingResponse>(recipe);
            return result;
        }

        public async Task<PagedResult<RatingDetailsResponse>> GetRatingDetailsAsync(Guid recipeId, RecipePaginationParams request, Guid? currentUserId = null)
        {
            var recipe = await _recipeRepository.GetByIdAsync(
                id: recipeId,
                include: i => i.Include(r => r.Ratings)
                               .ThenInclude(rt => rt.User)
                               .ThenInclude(u => u.Avatar));

            if (recipe == null || recipe.IsDeleted)
                throw new AppException(AppResponseCode.NOT_FOUND, "Công thức không tồn tại");

            var query = recipe.Ratings
                .Where(v => !v.Recipe.IsDeleted)
                .OrderByDescending(v => v.CreatedAtUtc)
                .AsQueryable();

            var totalCount = query.Count();

            var pagedRatings = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var result = _mapper.Map<IEnumerable<RatingDetailsResponse>>(pagedRatings);

            // Set IsOwner for each rating
            if (currentUserId.HasValue)
            {
                foreach (var rating in result)
                {
                    rating.IsOwner = rating.UserInteractionResponse.Id == currentUserId.Value;
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

        public async Task<PagedResult<RecipeResponse>> GetHistoryAsync(Guid userId, RecipePaginationParams request)
        {
            var user = await _userRepository.GetByIdAsync(
                id: userId,
                include: i => i.Include(u => u.ViewedRecipes)
                               .ThenInclude(v => v.Recipe)
                                   .ThenInclude(r => r.Image)
                               .Include(u => u.ViewedRecipes)
                                   .ThenInclude(v => v.Recipe)
                                       .ThenInclude(r => r.Author)
                                           .ThenInclude(a => a.Avatar));

            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var query = user.ViewedRecipes
                .Where(v => !v.Recipe.IsDeleted)
                .OrderByDescending(v => v.ViewedAtUtc)
                .Select(v => v.Recipe)
                .AsQueryable();

            var totalCount = query.Count();

            var pagedRecipes = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var result = _mapper.Map<IEnumerable<RecipeResponse>>(pagedRecipes);

            return new PagedResult<RecipeResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
