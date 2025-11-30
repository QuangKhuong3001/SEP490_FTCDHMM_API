using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface;
using SEP490_FTCDHMM_API.Domain.Constants;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeIpm
{
    public class RecipeCommandService : IRecipeCommandService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly ILabelRepository _labelRepository;
        private readonly IRecipeUserTagRepository _recipeUserTagRepository;
        private readonly IDraftRecipeRepository _draftRecipeRepository;
        private readonly IUserFavoriteRecipeRepository _userFavoriteRecipeRepository;
        private readonly IUserSaveRecipeRepository _userSaveRecipeRepository;
        private readonly IRecipeValidationService _validator;
        private readonly IRecipeImageService _imageService;
        private readonly IRecipeNutritionService _nutritionService;
        private readonly IRecipeBehaviorService _behaviorService;
        private readonly IRecipeIngredientRepository _recipeIngredientRepository;

        public RecipeCommandService(
            IRecipeRepository recipeRepository,
            ILabelRepository labelRepository,
            IRecipeUserTagRepository recipeUserTagRepository,
            IDraftRecipeRepository draftRecipeRepository,
            IUserFavoriteRecipeRepository userFavoriteRecipeRepository,
            IUserSaveRecipeRepository userSaveRecipeRepository,
            IRecipeValidationService validator,
            IRecipeIngredientRepository recipeIngredientRepository,
            IRecipeImageService imageService,
            IRecipeNutritionService nutritionService,
            IRecipeBehaviorService behaviorService)
        {
            _recipeRepository = recipeRepository;
            _labelRepository = labelRepository;
            _recipeUserTagRepository = recipeUserTagRepository;
            _draftRecipeRepository = draftRecipeRepository;
            _recipeIngredientRepository = recipeIngredientRepository;
            _userFavoriteRecipeRepository = userFavoriteRecipeRepository;
            _userSaveRecipeRepository = userSaveRecipeRepository;
            _validator = validator;
            _imageService = imageService;
            _nutritionService = nutritionService;
            _behaviorService = behaviorService;
        }

        public async Task CreateRecipeAsync(Guid userId, CreateRecipeRequest request)
        {
            var description = string.IsNullOrWhiteSpace(request.Description)
                ? DefaultValues.DEFAULT_RECIPE_DESCRIPTION
                : request.Description.Trim();

            await _validator.ValidateUserExistsAsync(userId);
            await _validator.ValidateLabelsAsync(request.LabelIds);
            await _validator.ValidateIngredientsAsync(request.Ingredients.Select(i => i.IngredientId));
            await _validator.ValidateCookingStepsAsync(request.CookingSteps);
            await _validator.ValidateTaggedUsersAsync(userId, request.TaggedUserIds);

            var draftExist = await _draftRecipeRepository.GetDraftByAuthorIdAsync(userId);
            if (draftExist != null)
                await _draftRecipeRepository.DeleteAsync(draftExist);

            var labels = await _labelRepository.GetAllAsync(l => request.LabelIds.Contains(l.Id));

            var recipe = new Recipe
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = description,
                AuthorId = userId,
                Difficulty = DifficultyValue.From(request.Difficulty),
                CookTime = request.CookTime,
                Labels = labels.ToList(),
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow,
                Ration = request.Ration,
                RecipeIngredients = request.Ingredients.Select(i => new RecipeIngredient
                {
                    IngredientId = i.IngredientId,
                    QuantityGram = i.QuantityGram
                }).ToList()
            };

            if (request.TaggedUserIds.Any())
            {
                foreach (var tagUserId in request.TaggedUserIds.Distinct())
                {
                    recipe.RecipeUserTags.Add(new RecipeUserTag
                    {
                        RecipeId = recipe.Id,
                        TaggedUserId = tagUserId
                    });
                }
            }

            await _imageService.SetRecipeImageAsync(recipe, request.Image, userId);

            var steps = await _imageService.CreateCookingStepsAsync(request.CookingSteps, recipe, userId);
            recipe.CookingSteps = steps;

            await _recipeRepository.AddAsync(recipe);

            var fullRecipe = await _recipeRepository.GetByIdAsync(recipe.Id,
                include: q => q
                    .Include(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
                            .ThenInclude(i => i.IngredientNutrients)
                                .ThenInclude(n => n.Nutrient)
            );

            await _nutritionService.AggregateAsync(fullRecipe!);
        }

        public async Task UpdateRecipeAsync(Guid userId, Guid recipeId, UpdateRecipeRequest request)
        {
            var description = string.IsNullOrWhiteSpace(request.Description)
                ? DefaultValues.DEFAULT_RECIPE_DESCRIPTION
                : request.Description.Trim();

            await _validator.ValidateUserExistsAsync(userId);
            await _validator.ValidateLabelsAsync(request.LabelIds);
            await _validator.ValidateIngredientsAsync(request.Ingredients.Select(i => i.IngredientId));
            await _validator.ValidateCookingStepsAsync(request.CookingSteps);
            await _validator.ValidateTaggedUsersAsync(userId, request.TaggedUserIds);

            var recipe = await _recipeRepository.GetByIdAsync(
                id: recipeId,
                include: q => q
                    .Include(r => r.Labels)
                    .Include(r => r.RecipeIngredients)
                    .Include(r => r.RecipeUserTags)
            );

            if (recipe == null || recipe.IsDeleted)
                throw new AppException(AppResponseCode.NOT_FOUND);

            await _validator.ValidateRecipeOwnerAsync(userId, recipe);

            recipe.Name = request.Name;
            recipe.Description = description;
            recipe.Difficulty = DifficultyValue.From(request.Difficulty);
            recipe.CookTime = request.CookTime;
            recipe.Ration = request.Ration;
            recipe.UpdatedAtUtc = DateTime.UtcNow;

            var labels = await _labelRepository.GetAllAsync(l => request.LabelIds.Contains(l.Id));
            recipe.Labels.Clear();
            recipe.Labels = labels.ToList();

            var oldIngredients = recipe.RecipeIngredients.ToList();
            if (oldIngredients.Any())
                await _recipeIngredientRepository.DeleteRangeAsync(oldIngredients);

            recipe.RecipeIngredients = request.Ingredients.Select(i => new RecipeIngredient
            {
                RecipeId = recipeId,
                IngredientId = i.IngredientId,
                QuantityGram = i.QuantityGram
            }).ToList();

            await _imageService.ReplaceRecipeImageAsync(recipe, request.Image, userId);

            var oldTags = await _recipeUserTagRepository.GetAllAsync(t => t.RecipeId == recipe.Id);
            if (oldTags.Any())
                await _recipeUserTagRepository.DeleteRangeAsync(oldTags);

            if (request.TaggedUserIds.Any())
            {
                foreach (var userTagId in request.TaggedUserIds.Distinct())
                {
                    recipe.RecipeUserTags.Add(new RecipeUserTag
                    {
                        RecipeId = recipe.Id,
                        TaggedUserId = userTagId
                    });
                }
            }

            await _imageService.ReplaceCookingStepsAsync(recipe.Id, request.CookingSteps, userId);

            await _recipeRepository.UpdateAsync(recipe);

            var fullRecipe = await _recipeRepository.GetByIdAsync(
                recipe.Id,
                include: q => q
                    .Include(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
                            .ThenInclude(i => i.IngredientNutrients)
                                .ThenInclude(n => n.Nutrient)
            );

            await _nutritionService.AggregateAsync(fullRecipe!);
        }


        public async Task DeleteRecipeAsync(Guid userId, Guid recipeId)
        {
            await _validator.ValidateUserExistsAsync(userId);

            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null || recipe.IsDeleted)
                throw new AppException(AppResponseCode.NOT_FOUND);

            await _validator.ValidateRecipeOwnerAsync(userId, recipe);

            recipe.IsDeleted = true;
            await _recipeRepository.UpdateAsync(recipe);
        }

        public async Task AddToFavoriteAsync(Guid userId, Guid recipeId)
        {
            await _validator.ValidateUserExistsAsync(userId);

            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null || recipe.IsDeleted)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var exist = await _userFavoriteRecipeRepository.ExistsAsync(f => f.UserId == userId && f.RecipeId == recipeId);
            if (exist)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            await _userFavoriteRecipeRepository.AddAsync(new UserFavoriteRecipe
            {
                RecipeId = recipeId,
                UserId = userId
            });

            await _behaviorService.RecordFarvoriteAsync(userId, recipeId);
        }

        public async Task RemoveFromFavoriteAsync(Guid userId, Guid recipeId)
        {
            await _validator.ValidateUserExistsAsync(userId);

            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null || recipe.IsDeleted)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var exist = await _userFavoriteRecipeRepository.GetAllAsync(f => f.UserId == userId && f.RecipeId == recipeId);
            if (!exist.Any())
                throw new AppException(AppResponseCode.INVALID_ACTION);

            await _userFavoriteRecipeRepository.DeleteAsync(exist.First());
            await _behaviorService.RecordUnFavoriteAsync(userId, recipeId);
        }

        public async Task SaveRecipeAsync(Guid userId, Guid recipeId)
        {
            await _validator.ValidateUserExistsAsync(userId);

            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null || recipe.IsDeleted)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var exist = await _userSaveRecipeRepository.ExistsAsync(f => f.UserId == userId && f.RecipeId == recipeId);
            if (exist)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            await _userSaveRecipeRepository.AddAsync(new UserSaveRecipe
            {
                RecipeId = recipeId,
                UserId = userId
            });

            await _behaviorService.RecordSaveAsync(userId, recipeId);
        }

        public async Task UnsaveRecipeAsync(Guid userId, Guid recipeId)
        {
            await _validator.ValidateUserExistsAsync(userId);

            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null || recipe.IsDeleted)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var exist = await _userSaveRecipeRepository.GetAllAsync(f => f.UserId == userId && f.RecipeId == recipeId);
            if (!exist.Any())
                throw new AppException(AppResponseCode.INVALID_ACTION);

            await _userSaveRecipeRepository.DeleteAsync(exist.First());
            await _behaviorService.RecordUnsaveAsync(userId, recipeId);
        }

        public async Task CopyRecipe(Guid userId, Guid parentId, CopyRecipeRequest request)
        {
            var description = string.IsNullOrWhiteSpace(request.Description)
               ? DefaultValues.DEFAULT_RECIPE_DESCRIPTION
               : request.Description.Trim();

            await _validator.ValidateUserExistsAsync(userId);
            await _validator.ValidateLabelsAsync(request.LabelIds);
            await _validator.ValidateIngredientsAsync(request.Ingredients.Select(i => i.IngredientId));
            await _validator.ValidateCookingStepsAsync(request.CookingSteps);
            await _validator.ValidateTaggedUsersAsync(userId, request.TaggedUserIds);

            var parent = await _recipeRepository.GetByIdAsync(parentId);
            if (parent == null || parent.IsDeleted)
                throw new AppException(AppResponseCode.NOT_FOUND, "Công thức được sao chép không tồn tại");

            if (parent.ParentId.HasValue)
            {
                parentId = parent.ParentId.Value;
            }

            var draftExist = await _draftRecipeRepository.GetDraftByAuthorIdAsync(userId);
            if (draftExist != null)
                await _draftRecipeRepository.DeleteAsync(draftExist);

            var labels = await _labelRepository.GetAllAsync(l => request.LabelIds.Contains(l.Id));

            var recipe = new Recipe
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = description,
                AuthorId = userId,
                Difficulty = DifficultyValue.From(request.Difficulty),
                CookTime = request.CookTime,
                Labels = labels.ToList(),
                Ration = request.Ration,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow,
                ParentId = parentId,
                RecipeIngredients = request.Ingredients.Select(i => new RecipeIngredient
                {
                    IngredientId = i.IngredientId,
                    QuantityGram = i.QuantityGram
                }).ToList()
            };

            if (request.TaggedUserIds.Any())
            {
                foreach (var tagUserId in request.TaggedUserIds.Distinct())
                {
                    recipe.RecipeUserTags.Add(new RecipeUserTag
                    {
                        RecipeId = recipe.Id,
                        TaggedUserId = tagUserId
                    });
                }
            }

            // If user uploaded a new image, use it; otherwise copy from parent recipe
            if (request.Image != null)
            {
                await _imageService.SetRecipeImageAsync(recipe, request.Image, userId);
            }
            else
            {
                await _imageService.CopyRecipeImageFromParentAsync(recipe, parent, userId);
            }

            var steps = await _imageService.CreateCookingStepsAsync(request.CookingSteps, recipe, userId);
            recipe.CookingSteps = steps;

            await _recipeRepository.AddAsync(recipe);

            var fullRecipe = await _recipeRepository.GetByIdAsync(recipe.Id,
                include: q => q
                    .Include(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
                            .ThenInclude(i => i.IngredientNutrients)
                                .ThenInclude(n => n.Nutrient)
            );

            await _nutritionService.AggregateAsync(fullRecipe!);
        }
    }
}
