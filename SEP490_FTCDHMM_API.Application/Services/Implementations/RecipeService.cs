using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.UserFavoriteRecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.UserSaveRecipeDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Interfaces;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper;
        private readonly IS3ImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;
        //private readonly ICacheService _cache;
        private readonly ILabelRepository _labelRepository;
        private readonly IUserRepository _userRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IUserRecipeViewRepository _userRecipeViewRepository;
        private readonly IUserFavoriteRecipeRepository _userFavoriteRecipeRepository;
        private readonly IUserSaveRecipeRepository _userSaveRecipeRepository;
        private readonly ICookingStepRepository _cookingStepRepository;
        private readonly IRecipeNutritionAggregator _recipeNutritionAggregator;

        public RecipeService(IRecipeRepository recipeRepository,
            IMapper mapper,
            IS3ImageService imageService,
            IUnitOfWork unitOfWork,
            //ICacheService cache,
            ILabelRepository labelRepository,
            IUserRepository userRepository,
            IIngredientRepository ingredientRepository,
            IUserRecipeViewRepository userRecipeViewRepository,
            IUserFavoriteRecipeRepository userFavoriteRecipeRepository,
            IUserSaveRecipeRepository userSaveRecipeRepository,
            ICookingStepRepository cookingStepRepository,
            IRecipeNutritionAggregator recipeNutritionAggregator)
        {
            _recipeRepository = recipeRepository;
            _mapper = mapper;
            _imageService = imageService;
            _unitOfWork = unitOfWork;
            //_cache = cache;
            _labelRepository = labelRepository;
            _userRepository = userRepository;
            _ingredientRepository = ingredientRepository;
            _userRecipeViewRepository = userRecipeViewRepository;
            _userFavoriteRecipeRepository = userFavoriteRecipeRepository;
            _userSaveRecipeRepository = userSaveRecipeRepository;
            _cookingStepRepository = cookingStepRepository;
            _recipeNutritionAggregator = recipeNutritionAggregator;
        }

        public async Task CreatRecipe(Guid userId, CreateRecipeRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var labelExists = await _labelRepository.IdsExistAsync(request.LabelIds);

            var ingredientIds = request.Ingredients.Select(i => i.IngredientId).ToList();
            var ingredientExists = await _ingredientRepository.IdsExistAsync(ingredientIds);

            if (!(labelExists && ingredientExists))
                throw new AppException(AppResponseCode.INVALID_ACTION);

            var labels = await _labelRepository.GetAllAsync(l => request.LabelIds.Contains(l.Id));

            var recipe = new Recipe
            {
                Name = request.Name,
                Description = request.Description,
                AuthorId = userId,
                Difficulty = DifficultyValue.From(request.Difficulty),
                CookTime = request.CookTime,
                Labels = labels.ToList(),
                Ration = request.Ration,
                RecipeIngredients = request.Ingredients.Select(i => new RecipeIngredient
                {
                    IngredientId = i.IngredientId,
                    QuantityGram = i.QuantityGram
                }).ToList()
            };


            if (request.Image != null && request.Image.Length > 0)
            {
                var image = await _imageService.UploadImageAsync(request.Image, StorageFolder.RECIPES, user);
                recipe.Image = image;
            }

            var steps = new List<CookingStep>();

            foreach (var step in request.CookingSteps.OrderBy(s => s.StepOrder))
            {
                if (string.IsNullOrWhiteSpace(step.Instruction))
                    throw new AppException(AppResponseCode.INVALID_ACTION);

                Image? stepImage = null;
                if (step.Image != null)
                    stepImage = await _imageService.UploadImageAsync(step.Image, StorageFolder.COOKING_STEPS, user);

                steps.Add(new CookingStep
                {
                    Id = Guid.NewGuid(),
                    Instruction = step.Instruction.Trim(),
                    StepOrder = step.StepOrder,
                    Image = stepImage,
                    Recipe = recipe
                });

            }
            recipe.CookingSteps = steps;

            await _recipeRepository.AddAsync(recipe);

            var fullRecipe = await _recipeRepository.GetByIdAsync(recipe.Id,
            include: q => q
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                        .ThenInclude(i => i.IngredientNutrients)
                        .ThenInclude(n => n.Nutrient)
);
            await _recipeNutritionAggregator.AggregateAndSaveAsync(fullRecipe!);
        }
        public async Task UpdateRecipe(Guid userId, Guid recipeId, UpdateRecipeRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var recipe = await _recipeRepository.GetByIdAsync(recipeId, include: i => i.Include(r => r.Labels).Include(r => r.RecipeIngredients));
            if ((recipe == null) || (recipe.isDeleted == true))
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (recipe.AuthorId != userId)
            {
                throw new AppException(AppResponseCode.FORBIDDEN);
            }

            var labelExists = await _labelRepository.IdsExistAsync(request.LabelIds);

            var ingredientIds = request.Ingredients.Select(i => i.IngredientId).ToList();
            var ingredientExists = await _ingredientRepository.IdsExistAsync(ingredientIds);

            if (!(labelExists && ingredientExists))
                throw new AppException(AppResponseCode.INVALID_ACTION);

            var labels = await _labelRepository.GetAllAsync(l => request.LabelIds.Contains(l.Id));

            recipe.Labels.Clear();
            recipe.RecipeIngredients.Clear();

            recipe.Name = request.Name;
            recipe.Description = request.Description;
            recipe.Difficulty = DifficultyValue.From(request.Difficulty);
            recipe.CookTime = request.CookTime;
            recipe.Labels = labels.ToList();
            recipe.RecipeIngredients = request.Ingredients.Select(i => new RecipeIngredient
            {
                RecipeId = recipeId,
                IngredientId = i.IngredientId,
                QuantityGram = i.QuantityGram
            }).ToList();
            recipe.UpdatedAtUtc = DateTime.UtcNow;
            recipe.Ration = request.Ration;

            if (request.Image != null && request.Image.Length > 0)
            {
                if (recipe.ImageId.HasValue)
                {
                    await _imageService.DeleteImageAsync(recipe.ImageId.Value);
                }

                var image = await _imageService.UploadImageAsync(request.Image, StorageFolder.RECIPES, user);
                recipe.Image = image;
            }

            foreach (var oldStep in recipe.CookingSteps)
            {
                if (oldStep.ImageId.HasValue)
                    await _imageService.DeleteImageAsync(oldStep.ImageId.Value);
                await _cookingStepRepository.DeleteAsync(oldStep);
            }

            var newSteps = new List<CookingStep>();
            foreach (var step in request.CookingSteps.OrderBy(s => s.StepOrder))
            {
                if (string.IsNullOrWhiteSpace(step.Instruction))
                    throw new AppException(AppResponseCode.INVALID_ACTION);

                Image? stepImage = null;

                if (step.Image != null && step.Image.Length > 0)
                {
                    stepImage = await _imageService.UploadImageAsync(step.Image, StorageFolder.COOKING_STEPS, user);
                }

                newSteps.Add(new CookingStep
                {
                    Id = Guid.NewGuid(),
                    Instruction = step.Instruction.Trim(),
                    StepOrder = step.StepOrder,
                    Image = stepImage,
                    Recipe = recipe
                });

            }

            recipe.CookingSteps = newSteps;

            await _recipeRepository.UpdateAsync(recipe);

            await _recipeNutritionAggregator.AggregateAndSaveAsync(recipe);

        }
        public async Task DeleteRecipe(Guid userId, Guid recipeId)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if ((recipe == null) || (recipe.isDeleted == true))
                throw new AppException(AppResponseCode.NOT_FOUND);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (recipe.AuthorId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN);

            recipe.isDeleted = true;
            await _recipeRepository.UpdateAsync(recipe);
        }
        public async Task<PagedResult<RecipeResponse>> GetAllRecipes(RecipeFilterRequest request)
        {
            Expression<Func<Recipe, bool>> filter = f =>
                !f.isDeleted
                && (request.LabelId == null || f.Labels.Any(l => l.Id == request.LabelId))
                && (string.IsNullOrEmpty(request.Keyword) || f.Name.Contains(request.Keyword));

            Func<IQueryable<Recipe>, IOrderedQueryable<Recipe>>? orderBy = request.SortBy?.ToLower() switch
            {
                "name_asc" => q => q.OrderBy(r => r.Name),
                "name_desc" => q => q.OrderByDescending(r => r.Name),
                "time_asc" => q => q.OrderBy(r => r.CookTime),
                "time_desc" => q => q.OrderByDescending(r => r.CookTime),
                "latest" => q => q.OrderByDescending(r => r.UpdatedAtUtc),
                _ => q => q.OrderByDescending(r => r.UpdatedAtUtc)
            };

            string[]? searchProps = new[] { "Name", "Description" };

            Func<IQueryable<Recipe>, IQueryable<Recipe>>? include = q =>
                q.Include(r => r.Author)
                 .Include(r => r.Image)
                 .Include(r => r.RecipeIngredients)
                 .Include(r => r.Labels)
                 .Include(r => r.CookingSteps)
                 .ThenInclude(cs => cs.Image);

            var (items, totalCount) = await _recipeRepository.GetPagedAsync(
                pageNumber: request.PaginationParams.PageNumber,
                pageSize: request.PaginationParams.PageSize,
                filter: filter,
                orderBy: orderBy,
                keyword: request.Keyword,
                searchProperties: searchProps,
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

        public async Task<RecipeDetailsResponse> GetRecipeDetails(Guid userId, Guid recipeId)
        {
            IQueryable<Recipe> include(IQueryable<Recipe> q) =>
                q.Include(r => r.Author)
                 .Include(r => r.Author!.Avatar)
                 .Include(r => r.Image)
                 .Include(r => r.Labels)
                 .Include(r => r.CookingSteps)
                 .ThenInclude(cs => cs.Image)
                 .Include(r => r.RecipeIngredients)
                 .ThenInclude(ri => ri.Ingredient);

            var recipe = await _recipeRepository.GetByIdAsync(recipeId, include);

            if ((recipe == null) || (recipe.isDeleted))
                throw new AppException(AppResponseCode.NOT_FOUND);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var viewed = await _userRecipeViewRepository.GetAllAsync(v => v.UserId == userId && v.RecipeId == recipeId);

            if (viewed == null)
            {
                await _userRecipeViewRepository.AddAsync(new UserRecipeView
                {
                    RecipeId = recipeId,
                    UserId = userId,
                });
            }

            // Check if recipe is favorited and saved by current user
            var isFavorited = await _userFavoriteRecipeRepository.ExistsAsync(f => f.UserId == userId && f.RecipeId == recipeId);
            var isSaved = await _userSaveRecipeRepository.ExistsAsync(s => s.UserId == userId && s.RecipeId == recipeId);

            var result = _mapper.Map<RecipeDetailsResponse>(recipe);
            result.IsFavorited = isFavorited;
            result.IsSaved = isSaved;

            return result;
        }
        public async Task AddToFavorite(Guid userId, Guid recipeId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);

            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (recipe == null || recipe.isDeleted == true)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var exist = await _userFavoriteRecipeRepository.ExistsAsync(f => f.UserId == userId && f.RecipeId == recipeId);
            if (exist)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            await _userFavoriteRecipeRepository.AddAsync(
                new UserFavoriteRecipe
                {
                    RecipeId = recipeId,
                    UserId = userId
                });
        }
        public async Task RemoveFromFavorite(Guid userId, Guid recipeId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);

            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (recipe == null || recipe.isDeleted == true)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var exist = await _userFavoriteRecipeRepository.GetAllAsync(f => f.UserId == userId && f.RecipeId == recipeId);
            if (!exist.Any())
                throw new AppException(AppResponseCode.INVALID_ACTION);

            await _userFavoriteRecipeRepository.DeleteAsync(exist.First());
        }

        public async Task<PagedResult<RecipeResponse>> GetFavoriteList(Guid userId, FavoriteRecipeFilterRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var (items, totalCount) = await _userFavoriteRecipeRepository.GetPagedAsync(
                pageNumber: request.PaginationParams.PageNumber,
                pageSize: request.PaginationParams.PageSize,
                filter: f => f.UserId == userId && f.Recipe.isDeleted == false,
                orderBy: q => q.OrderByDescending(f => f.CreatedAtUtc),
                include: q => q
                    .Include(f => f.Recipe)
                        .ThenInclude(r => r.Author)
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

        public async Task<PagedResult<RecipeResponse>> GetSavedList(Guid userId, SaveRecipeFilterRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var (items, totalCount) = await _userSaveRecipeRepository.GetPagedAsync(
                pageNumber: request.PaginationParams.PageNumber,
                pageSize: request.PaginationParams.PageSize,
                filter: f => f.UserId == userId && f.Recipe.isDeleted == false,
                orderBy: q => q.OrderByDescending(f => f.CreatedAtUtc),
                include: q => q
                    .Include(f => f.Recipe)
                        .ThenInclude(r => r.Author)
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
        public async Task SaveRecipe(Guid userId, Guid recipeId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);

            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (recipe == null || recipe.isDeleted == true)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var exist = await _userSaveRecipeRepository.ExistsAsync(f => f.UserId == userId && f.RecipeId == recipeId);
            if (exist)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            await _userSaveRecipeRepository.AddAsync(
                new UserSaveRecipe
                {
                    RecipeId = recipeId,
                    UserId = userId
                });
        }
        public async Task UnsaveRecipe(Guid userId, Guid recipeId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);

            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (recipe == null || recipe.isDeleted == true)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var exist = await _userSaveRecipeRepository.GetAllAsync(f => f.UserId == userId && f.RecipeId == recipeId);
            if (!exist.Any())
                throw new AppException(AppResponseCode.INVALID_ACTION);

            await _userSaveRecipeRepository.DeleteAsync(exist.First());
        }

        public async Task<PagedResult<MyRecipeResponse>> GetRecipeByUserId(Guid userId, PaginationParams paginationParams)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            Func<IQueryable<Recipe>, IQueryable<Recipe>>? include = q =>
                 q.Include(r => r.Image)
                 .Include(r => r.Labels)
                 .Include(r => r.RecipeIngredients)
                 .ThenInclude(ri => ri.Ingredient)
                 .Include(r => r.CookingSteps)
                    .ThenInclude(cs => cs.Image);

            var (items, totalCount) = await _recipeRepository.GetPagedAsync(
                pageNumber: paginationParams.PageNumber,
                pageSize: paginationParams.PageSize,
                filter: f => !f.isDeleted && f.AuthorId == userId,
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
    }
}
