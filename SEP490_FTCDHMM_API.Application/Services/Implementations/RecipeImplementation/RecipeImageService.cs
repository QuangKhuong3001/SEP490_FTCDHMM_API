using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep.CookingStepImage;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation
{
    public class RecipeImageService : IRecipeImageService
    {
        private readonly IS3ImageService _imageService;
        private readonly ICookingStepRepository _cookingStepRepository;
        private readonly IImageRepository _imageRepository;

        public RecipeImageService(
            IS3ImageService imageService,
            ICookingStepRepository cookingStepRepository,
            IImageRepository imageRepository)
        {
            _imageService = imageService;
            _cookingStepRepository = cookingStepRepository;
            _imageRepository = imageRepository;
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

        public async Task SetRecipeImageFromUrlAsync(Recipe recipe, string? existingImageUrl, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(existingImageUrl))
                return;

            var mirrored = await _imageService.MirrorExternalImageAsync(
                StorageFolder.RECIPES,
                existingImageUrl,
                userId
            );

            recipe.Image = mirrored;
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

        public async Task<List<CookingStep>> CreateCookingStepsAsync(IEnumerable<CookingStepRequest> steps, Recipe recipe, Guid userId)
        {
            var result = new List<CookingStep>();

            foreach (var step in steps.OrderBy(s => s.StepOrder))
            {
                var newStep = new CookingStep
                {
                    Id = Guid.NewGuid(),
                    Instruction = step.Instruction.Trim(),
                    StepOrder = step.StepOrder,
                    RecipeId = recipe.Id
                };

                var imageRequests = step.Images?.ToList() ?? new List<CookingStepImageRequest>();
                if (imageRequests.Any())
                {
                    newStep.CookingStepImages = new List<CookingStepImage>();

                    foreach (var img in imageRequests)
                    {
                        Guid imageId;

                        if (img.ExistingImageId.HasValue)
                        {
                            var existingImage = await _imageRepository.GetByIdAsync(img.ExistingImageId.Value);
                            if (existingImage == null)
                            {
                                continue;
                            }
                            imageId = existingImage.Id;
                        }
                        else if (img.Image != null)
                        {
                            var uploaded = await _imageService.UploadImageAsync(
                                img.Image,
                                StorageFolder.COOKING_STEPS,
                                userId
                            );
                            imageId = uploaded.Id;
                        }
                        else
                        {
                            continue;
                        }

                        newStep.CookingStepImages.Add(new CookingStepImage
                        {
                            Id = Guid.NewGuid(),
                            CookingStepId = newStep.Id,
                            ImageOrder = img.ImageOrder,
                            ImageId = imageId
                        });
                    }
                }

                result.Add(newStep);
            }

            return result;
        }

        public async Task ReplaceCookingStepsAsync(Guid recipeId, IEnumerable<CookingStepRequest> newSteps, Guid userId)
        {
            var oldSteps = await _cookingStepRepository.GetAllAsync(r => r.RecipeId == recipeId,
                include: q => q
                    .Include(s => s.CookingStepImages));

            foreach (var oldStep in oldSteps)
            {
                foreach (var si in oldStep.CookingStepImages)
                {
                    await _imageService.DeleteImageAsync(si.ImageId);
                }
            }

            await _cookingStepRepository.DeleteStepsByRecipeIdAsync(recipeId);

            var newStepEntities = new List<CookingStep>();

            foreach (var step in newSteps.OrderBy(s => s.StepOrder))
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
                            ImageId = uploaded.Id
                        });
                    }
                }

                newStepEntities.Add(newStep);
            }

            await _cookingStepRepository.AddRangeAsync(newStepEntities);
        }


    }
}
