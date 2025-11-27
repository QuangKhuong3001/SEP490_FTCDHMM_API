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
        private readonly IUserRepository _userRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IRecipeUserTagRepository _recipeUserTagRepository;
        private readonly IDraftRecipeRepository _draftRecipeRepository;
        private readonly IUserFavoriteRecipeRepository _userFavoriteRecipeRepository;
        private readonly IUserSaveRecipeRepository _userSaveRecipeRepository;
        private readonly IRecipeValidationService _validator;
        private readonly IRecipeImageService _imageService;
        private readonly IRecipeNutritionService _nutritionService;
        private readonly IRecipeBehaviorService _behaviorService;

        public RecipeCommandService(
            IRecipeRepository recipeRepository,
            ILabelRepository labelRepository,
            IUserRepository userRepository,
            IIngredientRepository ingredientRepository,
            IRecipeUserTagRepository recipeUserTagRepository,
            IDraftRecipeRepository draftRecipeRepository,
            IUserFavoriteRecipeRepository userFavoriteRecipeRepository,
            IUserSaveRecipeRepository userSaveRecipeRepository,
            IRecipeValidationService validator,
            IRecipeImageService imageService,
            IRecipeNutritionService nutritionService,
            IRecipeBehaviorService behaviorService)
        {
            _recipeRepository = recipeRepository;
            _labelRepository = labelRepository;
            _userRepository = userRepository;
            _ingredientRepository = ingredientRepository;
            _recipeUserTagRepository = recipeUserTagRepository;
            _draftRecipeRepository = draftRecipeRepository;
            _userFavoriteRecipeRepository = userFavoriteRecipeRepository;
            _userSaveRecipeRepository = userSaveRecipeRepository;
            _validator = validator;
            _imageService = imageService;
            _nutritionService = nutritionService;
            _behaviorService = behaviorService;
        }

        public async Task<Guid> CreateRecipeAsync(Guid userId, CreateRecipeRequest request)
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

            var steps = await _imageService.CreateCookingStepsAsync(request.CookingSteps, recipe.Id, userId);
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

            return recipe.Id;
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

            var labels = await _labelRepository.GetAllAsync(l => request.LabelIds.Contains(l.Id));

            recipe.Name = request.Name;
            recipe.Description = description;
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

            await _imageService.ReplaceRecipeImageAsync(recipe, request.Image, userId);

            var existingTags = await _recipeUserTagRepository.GetAllAsync(t => t.RecipeId == recipe.Id);
            if (existingTags.Any())
                await _recipeUserTagRepository.DeleteRangeAsync(existingTags);

            if (request.TaggedUserIds.Any())
            {
                foreach (var tagUserId in request.TaggedUserIds.Distinct())
                {
                    recipe.RecipeUserTags.Add(new RecipeUserTag
                    {
                        RecipeId = recipe.Id,
                        TaggedUserId = tagUserId,
                    });
                }
            }

            // Update recipe first (without cooking steps in the change tracker)
            await _recipeRepository.UpdateAsync(recipe);

            // Replace cooking steps separately after recipe is saved
            await _imageService.ReplaceCookingStepsAsync(recipeId, request.CookingSteps, userId);

            // Reload recipe with cooking steps for nutrition aggregation
            var updatedRecipe = await _recipeRepository.GetByIdAsync(recipeId,
                include: q => q
                    .Include(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
                            .ThenInclude(i => i.IngredientNutrients)
                                .ThenInclude(n => n.Nutrient)
                    .Include(r => r.CookingSteps)
            );

            if (updatedRecipe != null)
            {
                await _nutritionService.AggregateAsync(updatedRecipe);
            }
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
    }
}
