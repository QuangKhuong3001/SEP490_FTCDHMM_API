using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces;
using SEP490_FTCDHMM_API.Domain.Constants;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation
{
    public class RecipeCommandService : IRecipeCommandService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly ILabelRepository _labelRepository;
        private readonly IRecipeUserTagRepository _recipeUserTagRepository;
        private readonly IDraftRecipeRepository _draftRecipeRepository;
        private readonly IUserSaveRecipeRepository _userSaveRecipeRepository;
        private readonly IRecipeValidationService _validator;
        private readonly IRecipeImageService _imageService;
        private readonly IRecipeNutritionService _nutritionService;
        private readonly IRecipeIngredientRepository _recipeIngredientRepository;
        private readonly ICacheService _cacheService;
        private readonly IUserFollowRepository _userFollowRepository;
        private readonly INotificationCommandService _notificationCommandService;
        public RecipeCommandService(
            IRecipeRepository recipeRepository,
            ILabelRepository labelRepository,
            IRecipeUserTagRepository recipeUserTagRepository,
            IDraftRecipeRepository draftRecipeRepository,
            IUserSaveRecipeRepository userSaveRecipeRepository,
            IRecipeValidationService validator,
            ICacheService cacheService,
            IRecipeIngredientRepository recipeIngredientRepository,
            IRecipeImageService imageService,
            IUserFollowRepository userFollowRepository,
            INotificationCommandService notificationCommandService,
            IRecipeNutritionService nutritionService)
        {
            _recipeRepository = recipeRepository;
            _labelRepository = labelRepository;
            _recipeUserTagRepository = recipeUserTagRepository;
            _cacheService = cacheService;
            _draftRecipeRepository = draftRecipeRepository;
            _recipeIngredientRepository = recipeIngredientRepository;
            _userSaveRecipeRepository = userSaveRecipeRepository;
            _validator = validator;
            _notificationCommandService = notificationCommandService;
            _userFollowRepository = userFollowRepository;
            _imageService = imageService;
            _nutritionService = nutritionService;
        }

        private async Task CreateAndSendNotificationsAsync(Guid senderId, Guid targetId)
        {
            var followers = await _userFollowRepository.GetAllAsync(u => u.FolloweeId == senderId);

            foreach (var follow in followers)
            {
                await _notificationCommandService.CreateAndSendNotificationAsync(senderId, follow.FollowerId, NotificationType.NewRecipe, targetId);
            }
        }

        public async Task CreateRecipeAsync(Guid userId, CreateRecipeRequest request)
        {
            var description = string.IsNullOrWhiteSpace(request.Description)
                ? DefaultValues.DEFAULT_RECIPE_DESCRIPTION
                : request.Description.Trim();

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
                NormalizedName = request.Name.NormalizeVi(),
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

            await _imageService.SetRecipeImageAsync(recipe, request.Image, request.ExistingMainImageId);

            var steps = await _imageService.CreateCookingStepsAsync(request.CookingSteps, recipe);
            recipe.CookingSteps = steps;

            await _recipeRepository.AddAsync(recipe);

            var fullRecipe = await _recipeRepository.GetByIdAsync(recipe.Id,
                include: q => q
                    .Include(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
                            .ThenInclude(i => i.IngredientNutrients)
                                .ThenInclude(n => n.Nutrient)
            );

            await _nutritionService.AggregateRecipeAsync(fullRecipe!);
            await _cacheService.RemoveByPrefixAsync("recipe");
            await this.CreateAndSendNotificationsAsync(userId, recipe.Id);
        }

        public async Task UpdateRecipeAsync(Guid userId, Guid recipeId, UpdateRecipeRequest request)
        {
            var description = string.IsNullOrWhiteSpace(request.Description)
                ? DefaultValues.DEFAULT_RECIPE_DESCRIPTION
                : request.Description.Trim();

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

            if (recipe == null || recipe.Status == RecipeStatus.Deleted)
                throw new AppException(AppResponseCode.NOT_FOUND);

            await _validator.ValidateRecipeOwnerAsync(userId, recipe);

            if (recipe.Status == RecipeStatus.Locked)
            {
                recipe.Status = RecipeStatus.Pending;
            }

            recipe.NormalizedName = request.Name.NormalizeVi();
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

            await _imageService.ReplaceRecipeImageAsync(recipe, request.Image);

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

            await _imageService.ReplaceCookingStepsAsync(recipe.Id, request.CookingSteps);

            await _recipeRepository.UpdateAsync(recipe);

            var fullRecipe = await _recipeRepository.GetByIdAsync(
                recipe.Id,
                include: q => q
                    .Include(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
                            .ThenInclude(i => i.IngredientNutrients)
                                .ThenInclude(n => n.Nutrient)
            );

            await _nutritionService.AggregateRecipeAsync(fullRecipe!);
            await _cacheService.RemoveByPrefixAsync("recipe");
        }


        public async Task DeleteRecipeAsync(Guid userId, Guid recipeId)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null || recipe.Status == RecipeStatus.Deleted)
                throw new AppException(AppResponseCode.NOT_FOUND);

            await _validator.ValidateRecipeOwnerAsync(userId, recipe);

            recipe.UpdatedAtUtc = DateTime.UtcNow;
            recipe.Status = RecipeStatus.Deleted;
            await _recipeRepository.UpdateAsync(recipe);
            await _cacheService.RemoveByPrefixAsync("recipe");
        }

        public async Task SaveRecipeAsync(Guid userId, Guid recipeId)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null || recipe.Status != RecipeStatus.Posted)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var exist = await _userSaveRecipeRepository.ExistsAsync(f => f.UserId == userId && f.RecipeId == recipeId);
            if (exist)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            await _userSaveRecipeRepository.AddAsync(new RecipeUserSave
            {
                RecipeId = recipeId,
                UserId = userId
            });
        }

        public async Task UnsaveRecipeAsync(Guid userId, Guid recipeId)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null || recipe.Status != RecipeStatus.Posted)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var exist = await _userSaveRecipeRepository.GetAllAsync(f => f.UserId == userId && f.RecipeId == recipeId);
            if (!exist.Any())
                throw new AppException(AppResponseCode.INVALID_ACTION);

            await _userSaveRecipeRepository.DeleteAsync(exist.First());
        }

        public async Task CopyRecipeAsync(Guid userId, Guid parentId, CopyRecipeRequest request)
        {
            var description = string.IsNullOrWhiteSpace(request.Description)
               ? DefaultValues.DEFAULT_RECIPE_DESCRIPTION
               : request.Description.Trim();

            await _validator.ValidateLabelsAsync(request.LabelIds);
            await _validator.ValidateIngredientsAsync(request.Ingredients.Select(i => i.IngredientId));
            await _validator.ValidateCookingStepsAsync(request.CookingSteps);
            await _validator.ValidateTaggedUsersAsync(userId, request.TaggedUserIds);

            // Load parent with images and cooking steps for potential copying
            var parent = await _recipeRepository.GetByIdAsync(
                id: parentId,
                include: q => q
                    .Include(r => r.Image)
                    .Include(r => r.CookingSteps)
                        .ThenInclude(cs => cs.CookingStepImages)
            );
            if (parent == null || parent.Status != RecipeStatus.Posted)
                throw new AppException(AppResponseCode.NOT_FOUND, "Công thức được sao chép không tồn tại");

            Guid? parentIdToSet = parentId;

            if (parent.AuthorId == userId)
            {
                parentIdToSet = null;
            }

            if (parent.ParentId.HasValue)
            {
                parentIdToSet = parent.ParentId.Value;
            }

            var draftExist = await _draftRecipeRepository.GetDraftByAuthorIdAsync(userId);
            if (draftExist != null)
                await _draftRecipeRepository.DeleteAsync(draftExist);

            var labels = await _labelRepository.GetAllAsync(l => request.LabelIds.Contains(l.Id));

            var recipe = new Recipe
            {
                Id = Guid.NewGuid(),
                NormalizedName = request.Name.NormalizeVi(),
                Name = request.Name,
                Description = description,
                AuthorId = userId,
                Difficulty = DifficultyValue.From(request.Difficulty),
                CookTime = request.CookTime,
                Labels = labels.ToList(),
                Ration = request.Ration,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow,
                ParentId = parentIdToSet,
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

            // Handle main image
            if (request.Image != null)
            {
                // New image uploaded
                await _imageService.SetRecipeImageAsync(recipe, request.Image);
            }
            else if (request.CopyMainImageFromParent)
            {
                // Copy image from parent recipe
                await _imageService.CopyMainImageFromParentAsync(recipe, parent);
            }

            // Handle cooking steps
            List<CookingStep> steps;
            if (request.CopyStepImagesFromParent)
            {
                // Create steps with images copied from parent
                steps = await _imageService.CreateCookingStepsWithCopyFromParentAsync(
                    request.CookingSteps, 
                    recipe, 
                    parent.CookingSteps);
            }
            else
            {
                // Normal creation (new images only)
                steps = await _imageService.CreateCookingStepsAsync(request.CookingSteps, recipe);
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

            await _nutritionService.AggregateRecipeAsync(fullRecipe!);
        }
    }
}
