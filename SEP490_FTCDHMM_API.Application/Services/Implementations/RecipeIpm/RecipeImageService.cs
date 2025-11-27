using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep.CookingStepImage;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeIpm
{
    public class RecipeImageService : IRecipeImageService
    {
        private readonly IS3ImageService _imageService;
        private readonly ICookingStepRepository _cookingStepRepository;

        public RecipeImageService(
            IS3ImageService imageService,
            ICookingStepRepository cookingStepRepository)
        {
            _imageService = imageService;
            _cookingStepRepository = cookingStepRepository;
        }

        public async Task SetRecipeImageAsync(Recipe recipe, FileUploadModel? file, Guid userId)
        {
            if (file == null)
                return;

            var uploaded = await _imageService.UploadImageAsync(
                file,
                StorageFolder.RECIPES,
                userId
            );

            recipe.Image = uploaded;
        }

        public async Task ReplaceRecipeImageAsync(Recipe recipe, FileUploadModel? file, Guid userId)
        {
            if (file == null)
                return;

            if (recipe.ImageId.HasValue)
            {
                await _imageService.DeleteImageAsync(recipe.ImageId.Value);
            }

            var newImage = await _imageService.UploadImageAsync(
                file,
                StorageFolder.RECIPES,
                userId
            );

            recipe.Image = newImage;
        }

        public async Task<List<CookingStep>> CreateCookingStepsAsync(IEnumerable<CookingStepRequest> steps, Guid recipeId, Guid userId)
        {
            var result = new List<CookingStep>();

            foreach (var step in steps.OrderBy(s => s.StepOrder))
            {
                var newStep = new CookingStep
                {
                    Id = Guid.NewGuid(),
                    Instruction = step.Instruction.Trim(),
                    StepOrder = step.StepOrder,
                    RecipeId = recipeId
                };

                var imageRequests = step.Images?.ToList() ?? new List<CookingStepImageRequest>();
                if (imageRequests.Any())
                {
                    newStep.CookingStepImages = new List<CookingStepImage>();

                    foreach (var img in imageRequests)
                    {
                        var uploaded = await _imageService.UploadImageAsync(
                            img.Image,
                            StorageFolder.COOKING_STEPS,
                            userId
                        );

                        newStep.CookingStepImages.Add(new CookingStepImage
                        {
                            Id = Guid.NewGuid(),
                            CookingStepId = newStep.Id,
                            ImageOrder = img.ImageOrder,
                            Image = uploaded
                        });
                    }
                }

                result.Add(newStep);
            }

            return result;
        }

        public async Task ReplaceCookingStepsAsync(Guid recipeId, IEnumerable<CookingStepRequest> newSteps, Guid userId)
        {
            // Delete old cooking steps from database (cascade will delete images)
            await _cookingStepRepository.DeleteStepsByRecipeIdAsync(recipeId);

            // Create new steps if any provided
            if (newSteps == null || !newSteps.Any())
                return;

            var steps = await CreateCookingStepsAsync(newSteps, recipeId, userId);

            // Add new steps to repository to persist them
            if (steps?.Any() == true)
            {
                await _cookingStepRepository.AddRangeAsync(steps);
            }
        }
    }
}
