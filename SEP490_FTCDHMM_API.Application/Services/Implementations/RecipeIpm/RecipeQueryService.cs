using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Rating;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.UserSaveRecipe;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Specifications;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeIpm
{
    public class RecipeQueryService : IRecipeQueryService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserSaveRecipeRepository _userSaveRecipeRepository;
        private readonly IMapper _mapper;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly ILabelRepository _labelRepository;


        public RecipeQueryService(
            IRecipeRepository recipeRepository,
            IUserRepository userRepository,
            IUserSaveRecipeRepository userSaveRecipeRepository,
            IIngredientRepository ingredientRepository,
            ILabelRepository labelRepository,
            IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _userRepository = userRepository;
            _userSaveRecipeRepository = userSaveRecipeRepository;
            _mapper = mapper;
            _ingredientRepository = ingredientRepository;
            _labelRepository = labelRepository;
        }

        public async Task<PagedResult<RecipeResponse>> GetAllRecipesAsync(RecipeFilterRequest request)
        {
            var inIngredientExist = await _ingredientRepository.IdsExistAsync(request.IncludeIngredientIds);
            var exIngredientExist = await _ingredientRepository.IdsExistAsync(request.ExcludeIngredientIds);
            var inLabelExist = await _labelRepository.IdsExistAsync(request.IncludeLabelIds);
            var exLabelExist = await _labelRepository.IdsExistAsync(request.ExcludeLabelIds);

            if (inIngredientExist && exIngredientExist)
                throw new AppException(AppResponseCode.NOT_FOUND, "Một hoặc nhiều nguyên liệu không tồn tại trong hệ thống");

            if (inLabelExist && exLabelExist)
                throw new AppException(AppResponseCode.NOT_FOUND, "Một hoặc nhiều nhãn không tồn tại trong hệ thống");

            var spec = new RecipeBasicFilterSpec
            {
                IncludeIngredientIds = request.IncludeIngredientIds,
                ExcludeIngredientIds = request.ExcludeIngredientIds,
                IncludeLabelIds = request.IncludeLabelIds,
                ExcludeLabelIds = request.ExcludeLabelIds,
                Difficulty = request.Difficulty,
                Keyword = request.Keyword,
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

        public async Task<RecipeDetailsResponse> GetRecipeDetailsAsync(Guid userId, Guid recipeId)
        {
            IQueryable<Recipe> include(IQueryable<Recipe> q) =>
                q.Include(r => r.Author).ThenInclude(u => u.Avatar)
                 .Include(r => r.Image)
                 .Include(r => r.Labels)
                 .Include(r => r.RecipeUserTags).ThenInclude(cs => cs.TaggedUser)
                 .Include(r => r.CookingSteps).ThenInclude(cs => cs.CookingStepImages).ThenInclude(cs => cs.Image)
                 .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient);

            var recipe = await _recipeRepository.GetByIdAsync(recipeId, include);

            await this.ValidateAccessRecipeAsync(userId, recipe);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            await _recipeRepository.UpdateAsync(recipe!);

            var isSaved = await _userSaveRecipeRepository.ExistsAsync(s => s.UserId == userId && s.RecipeId == recipeId);

            var result = _mapper.Map<RecipeDetailsResponse>(recipe);
            result.IsSaved = isSaved;

            return result;
        }

        public async Task<PagedResult<RecipeResponse>> GetSavedListAsync(Guid userId, SaveRecipeFilterRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var (items, totalCount) = await _userSaveRecipeRepository.GetPagedAsync(
                pageNumber: request.PaginationParams.PageNumber,
                pageSize: request.PaginationParams.PageSize,
                filter: f => f.UserId == userId && f.Recipe.Status == RecipeStatus.Posted,
                orderBy: q => q.OrderByDescending(f => f.CreatedAtUtc),
                include: q => q
                    .Include(f => f.Recipe).ThenInclude(r => r.Author).ThenInclude(u => u.Avatar)
                    .Include(f => f.Recipe.Image)
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
                filter: f => f.Status == RecipeStatus.Posted && f.AuthorId == userId,
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
                filter: f => f.Status == RecipeStatus.Posted && f.AuthorId == user.Id,
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

        public async Task<RecipeRatingResponse> GetRecipeRatingAsync(Guid userId, Guid recipeId)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);

            await this.ValidateAccessRecipeAsync(userId, recipe);

            var result = _mapper.Map<RecipeRatingResponse>(recipe);
            return result;
        }

        public async Task<PagedResult<RatingDetailsResponse>> GetRatingDetailsAsync(Guid userId, Guid recipeId, RecipePaginationParams request)
        {
            var recipe = await _recipeRepository.GetByIdAsync(
                id: recipeId,
                include: i => i.Include(r => r.Ratings)
                               .ThenInclude(rt => rt.User)
                               .ThenInclude(u => u.Avatar));

            await this.ValidateAccessRecipeAsync(userId, recipe);

            var query = recipe!.Ratings
                .OrderByDescending(v => v.CreatedAtUtc)
                .AsQueryable();

            var totalCount = query.Count();

            var pagedRatings = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var result = _mapper.Map<IEnumerable<RatingDetailsResponse>>(pagedRatings);

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
                .Where(v => v.Recipe.Status == RecipeStatus.Posted)
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

        public async Task<PagedResult<RecipeManagementResponse>> GetPendingManagementListAsync(PaginationParams request)
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

        public async Task<PagedResult<RecipeManagementResponse>> GetPendingListAsync(Guid userId, PaginationParams request)
        {
            Func<IQueryable<Recipe>, IQueryable<Recipe>> include = q =>
                q.Include(r => r.Author).ThenInclude(u => u.Avatar)
                 .Include(r => r.Image)
                 .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
                 .Include(r => r.Labels)
                 .Include(r => r.RecipeUserTags).ThenInclude(cs => cs.TaggedUser)
                 .Include(r => r.CookingSteps).ThenInclude(cs => cs.CookingStepImages).ThenInclude(cs => cs.Image);

            var (items, totalCount) = await _recipeRepository.GetPagedAsync(
                filter: f => f.Status == RecipeStatus.Pending && f.AuthorId == userId,
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

        private Task ValidateAccessRecipeAsync(Guid userId, Recipe? recipe)
        {
            if (recipe == null || recipe.Status == RecipeStatus.Deleted)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (recipe.Status != RecipeStatus.Posted && recipe.AuthorId != userId)
                throw new AppException(AppResponseCode.NOT_FOUND);

            return Task.CompletedTask;
        }

    }
}
