using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RatingDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.UserFavoriteRecipe;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.UserSaveRecipe;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Interfaces;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;

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
        private readonly IRecipeUserTagRepository _recipeUserTagRepository;

        public RecipeService(IRecipeRepository recipeRepository,
            IMapper mapper,
            IS3ImageService imageService,
            IUnitOfWork unitOfWork,
            //ICacheService cache,
            ILabelRepository labelRepository,
            IRecipeUserTagRepository recipeUserTagRepository,
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
            _recipeUserTagRepository = recipeUserTagRepository;
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
            var stepOrders = request.CookingSteps.Select(x => x.StepOrder).ToList();

            if (stepOrders.Count != stepOrders.Distinct().Count())
            {
                throw new AppException(AppResponseCode.INVALID_ACTION, "Thứ tự của các bước nấu ăn không được trùng nhau.");
            }

            foreach (var step in request.CookingSteps)
            {
                var imageOrders = step.Images.Select(i => i.ImageOrder).ToList();

                if (imageOrders.Count != imageOrders.Distinct().Count())
                {
                    throw new AppException(AppResponseCode.INVALID_ACTION, "Thứ tự ảnh trong cùng một bước không được trùng nhau.");
                }
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var labelExists = await _labelRepository.IdsExistAsync(request.LabelIds);

            var ingredientIds = request.Ingredients.Select(i => i.IngredientId).ToList();
            var ingredientExists = await _ingredientRepository.IdsExistAsync(ingredientIds);

            if (!(ingredientExists))
                throw new AppException(AppResponseCode.NOT_FOUND, "Nguyên liệu không tồn tại");

            if (!(labelExists))
                throw new AppException(AppResponseCode.NOT_FOUND, "Nhãn dán không tồn tại");

            if (ingredientIds.HasDuplicate())
                throw new AppException(AppResponseCode.DUPLICATE, "Danh sách nguyên liệu bị trùng lặp");

            if (request.LabelIds.HasDuplicate())
                throw new AppException(AppResponseCode.DUPLICATE, "Danh sách nhãn dán bị trùng lặp");


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

            if (request.Image != null)
            {
                var uploaded = await _imageService.UploadImageAsync(
                    request.Image,
                    StorageFolder.RECIPES,
                    userId
                );
                recipe.Image = uploaded;
            }

            if (request.TaggedUserIds.Any())
            {
                var distinctIds = request.TaggedUserIds.Distinct().ToList();

                foreach (var userIdToTag in distinctIds)
                {
                    if (userIdToTag == userId)
                        throw new AppException(AppResponseCode.INVALID_ACTION, "Bạn không thể gắn thẻ chính mình.");

                    var exists = await _userRepository.ExistsAsync(u => u.Id == userIdToTag);
                    if (!exists)
                        throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION, "Người dùng được gắn thẻ không hợp lệ.");

                    recipe.RecipeUserTags.Add(new RecipeUserTag
                    {
                        RecipeId = recipe.Id,
                        TaggedUserId = userIdToTag,
                    });
                }
            }

            var steps = new List<CookingStep>();

            foreach (var step in request.CookingSteps.OrderBy(s => s.StepOrder))
            {
                if (string.IsNullOrWhiteSpace(step.Instruction))
                    throw new AppException(AppResponseCode.INVALID_ACTION);

                var cookingStep = new CookingStep
                {
                    Id = Guid.NewGuid(),
                    Instruction = step.Instruction.Trim(),
                    StepOrder = step.StepOrder,
                    Recipe = recipe
                };

                if (step.Images != null && step.Images.Any())
                {
                    cookingStep.CookingStepImages = new List<CookingStepImage>();

                    foreach (var file in step.Images)
                    {
                        var uploaded = await _imageService.UploadImageAsync(
                            file.Image,
                            StorageFolder.COOKING_STEPS,
                            userId
                        );

                        cookingStep.CookingStepImages.Add(new CookingStepImage
                        {
                            Id = Guid.NewGuid(),
                            CookingStepId = cookingStep.Id,
                            ImageOrder = file.ImageOrder,
                            Image = uploaded,
                        });
                    }
                }

                steps.Add(cookingStep);
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

            var recipe = await _recipeRepository.GetByIdAsync(
                id: recipeId,
                include: q => q
                    .Include(r => r.Labels)
                    .Include(r => r.RecipeIngredients)
                    .Include(r => r.CookingSteps)
                        .ThenInclude(cs => cs.CookingStepImages)
                            .ThenInclude(si => si.Image)
            );

            if (recipe == null || recipe.IsDeleted)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (recipe.AuthorId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN);

            var labelExists = await _labelRepository.IdsExistAsync(request.LabelIds);

            var ingredientIds = request.Ingredients.Select(i => i.IngredientId).ToList();
            var ingredientExists = await _ingredientRepository.IdsExistAsync(ingredientIds);

            if (!(ingredientExists))
                throw new AppException(AppResponseCode.NOT_FOUND, "Nguyên liệu không tồn tại");

            if (!(labelExists))
                throw new AppException(AppResponseCode.NOT_FOUND, "Nhãn dán không tồn tại");

            if (ingredientIds.HasDuplicate())
                throw new AppException(AppResponseCode.DUPLICATE, "Danh sách nguyên liệu bị trùng lặp");

            if (request.LabelIds.HasDuplicate())
                throw new AppException(AppResponseCode.DUPLICATE, "Danh sách nhãn dán bị trùng lặp");

            var labels = await _labelRepository.GetAllAsync(l => request.LabelIds.Contains(l.Id));

            recipe.Name = request.Name;
            recipe.Description = request.Description;
            recipe.Difficulty = DifficultyValue.From(request.Difficulty);
            recipe.CookTime = request.CookTime;
            recipe.Ration = request.Ration;
            recipe.UpdatedAtUtc = DateTime.UtcNow;

            recipe.Labels.Clear();
            recipe.Labels = labels.ToList();

            recipe.RecipeIngredients.Clear();
            recipe.RecipeIngredients = request.Ingredients.Select(i => new RecipeIngredient
            {
                RecipeId = recipeId,
                IngredientId = i.IngredientId,
                QuantityGram = i.QuantityGram
            }).ToList();

            if (request.Image != null)
            {
                if (recipe.ImageId.HasValue)
                {
                    await _imageService.DeleteImageAsync(recipe.ImageId.Value);
                }

                var newImage = await _imageService.UploadImageAsync(
                    request.Image,
                    StorageFolder.RECIPES,
                    userId
                );

                recipe.Image = newImage;
            }

            var existingTags = await _recipeUserTagRepository.GetAllAsync(t => t.RecipeId == recipe.Id);

            if (existingTags.Any())
            {
                await _recipeUserTagRepository.DeleteRangeAsync(existingTags);
            }

            if (request.TaggedUserIds.Any())
            {
                var distinctIds = request.TaggedUserIds.Distinct().ToList();

                foreach (var userIdToTag in distinctIds)
                {
                    if (userIdToTag == userId)
                        throw new AppException(AppResponseCode.INVALID_ACTION, "Bạn không thể tag chính mình.");

                    var exists = await _userRepository.ExistsAsync(u => u.Id == userIdToTag);
                    if (!exists)
                        throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION,
                            $"Người dùng {userIdToTag} không tồn tại.");

                    recipe.RecipeUserTags.Add(new RecipeUserTag
                    {
                        RecipeId = recipe.Id,
                        TaggedUserId = userIdToTag,
                    });
                }
            }

            foreach (var oldStep in recipe.CookingSteps)
            {
                foreach (var si in oldStep.CookingStepImages)
                {
                    await _imageService.DeleteImageAsync(si.ImageId);
                }
            }

            recipe.CookingSteps.Clear();
            await _cookingStepRepository.DeleteStepsByRecipeIdAsync(recipeId);


            var newSteps = new List<CookingStep>();

            foreach (var step in request.CookingSteps.OrderBy(s => s.StepOrder))
            {
                if (string.IsNullOrWhiteSpace(step.Instruction))
                    throw new AppException(AppResponseCode.INVALID_ACTION);

                var newStep = new CookingStep
                {
                    Id = Guid.NewGuid(),
                    Instruction = step.Instruction.Trim(),
                    StepOrder = step.StepOrder,
                    Recipe = recipe
                };

                if (step.Images != null && step.Images.Any())
                {
                    newStep.CookingStepImages = new List<CookingStepImage>();

                    foreach (var file in step.Images)
                    {
                        var uploaded = await _imageService.UploadImageAsync(
                            file.Image,
                            StorageFolder.COOKING_STEPS,
                            userId
                        );

                        newStep.CookingStepImages.Add(new CookingStepImage
                        {
                            Id = Guid.NewGuid(),
                            CookingStepId = newStep.Id,
                            ImageOrder = file.ImageOrder,
                            ImageId = uploaded.Id,
                        });
                    }
                }

                newSteps.Add(newStep);
            }

            recipe.CookingSteps = newSteps;

            await _recipeRepository.UpdateAsync(recipe);

            await _recipeNutritionAggregator.AggregateAndSaveAsync(recipe);
        }

        public async Task DeleteRecipe(Guid userId, Guid recipeId)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if ((recipe == null) || (recipe.IsDeleted == true))
                throw new AppException(AppResponseCode.NOT_FOUND);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (recipe.AuthorId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN);

            recipe.IsDeleted = true;
            await _recipeRepository.UpdateAsync(recipe);
        }
        public async Task<PagedResult<RecipeResponse>> GetAllRecipes(RecipeFilterRequest request)
        {
            Expression<Func<Recipe, bool>> filter = f =>
                !f.IsDeleted
                && (request.Difficulty == null ||
                    f.Difficulty == DifficultyValue.From(request.Difficulty))
                && (request.Ration == null ||
                    f.Ration == request.Ration)
                && (request.MaxCookTime == null ||
                    f.CookTime < request.MaxCookTime)
                && ((!request.LabelIds.Any()) ||
                    f.Labels.Any(l => request.LabelIds.Contains(l.Id)))
                && ((!request.IngredientIds.Any()) ||
                    f.RecipeIngredients.Any(ri => request.IngredientIds.Contains(ri.IngredientId)))
                && (string.IsNullOrEmpty(request.Keyword) ||
                    f.Name.Contains(request.Keyword));

            Func<IQueryable<Recipe>, IOrderedQueryable<Recipe>> orderBy = request.SortBy?.ToLower() switch
            {
                "name_asc" => q => q.OrderBy(r => r.Name),
                "name_desc" => q => q.OrderByDescending(r => r.Name),
                "time_asc" => q => q.OrderBy(r => r.CookTime),
                "time_desc" => q => q.OrderByDescending(r => r.CookTime),
                "latest" => q => q.OrderByDescending(r => r.UpdatedAtUtc),
                "rate_asc" => q => q.OrderBy(r => r.Rating),
                "rate_desc" => q => q.OrderByDescending(r => r.Rating),
                _ => q => q.OrderByDescending(r => r.UpdatedAtUtc)
            };

            Func<IQueryable<Recipe>, IQueryable<Recipe>> include = q =>
                q.Include(r => r.Author)
                 .Include(r => r.Image)
                 .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                 .Include(r => r.Labels)
                 .Include(r => r.CookingSteps)
                    .ThenInclude(cs => cs.CookingStepImages)
                        .ThenInclude(cs => cs.Image);

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


        public async Task<RecipeDetailsResponse> GetRecipeDetails(Guid userId, Guid recipeId)
        {
            IQueryable<Recipe> include(IQueryable<Recipe> q) =>
                q.Include(r => r.Author)
                 .Include(r => r.Author!.Avatar)
                 .Include(r => r.Image)
                 .Include(r => r.Labels)
                 .Include(r => r.CookingSteps)
                    .ThenInclude(cs => cs.CookingStepImages)
                        .ThenInclude(cs => cs.Image)
                 .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient);

            var recipe = await _recipeRepository.GetByIdAsync(recipeId, include);

            if ((recipe == null) || (recipe.IsDeleted))
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

            if (recipe == null || recipe.IsDeleted == true)
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

            if (recipe == null || recipe.IsDeleted == true)
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
                filter: f => f.UserId == userId && f.Recipe.IsDeleted == false,
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
                filter: f => f.UserId == userId && f.Recipe.IsDeleted == false,
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

            if (recipe == null || recipe.IsDeleted == true)
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

            if (recipe == null || recipe.IsDeleted == true)
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
                    .ThenInclude(cs => cs.CookingStepImages)
                        .ThenInclude(cs => cs.Image);

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

        public async Task<double> GetAverageScore(Guid recipeId)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null || recipe.IsDeleted)
                throw new AppException(AppResponseCode.NOT_FOUND, "Công thức không tồn tại");

            return recipe.Rating;
        }

        public async Task<PagedResult<RatingResponse>> GetRaiting(Guid recipeId, PaginationParams request)
        {
            var recipe = await _recipeRepository.GetByIdAsync(
                id: recipeId,
                include: i => i.Include(r => r.Ratings));

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


            var result = _mapper.Map<IEnumerable<RatingResponse>>(pagedRatings);
            return new PagedResult<RatingResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PagedResult<RecipeResponse>> GetHistory(Guid userId, PaginationParams request)
        {
            var user = await _userRepository.GetByIdAsync(
                id: userId,
                include: i => i.Include(u => u.ViewedRecipes)
                               .ThenInclude(v => v.Recipe)
                                   .ThenInclude(r => r.Image));

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
